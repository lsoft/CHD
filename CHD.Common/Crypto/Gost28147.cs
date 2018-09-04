using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace CHD.Common.Crypto
{

    public sealed class Gost28147 : ICrypto
    {
        private readonly uint[] _encodeKeyIndex =
        {
            0,1,2,3,4,5,6,7,
            0,1,2,3,4,5,6,7,
            0,1,2,3,4,5,6,7,
            7,6,5,4,3,2,1,0
        };

        private readonly uint[] _decodeKeyIndex =
        {
            0,1,2,3,4,5,6,7,
            7,6,5,4,3,2,1,0,
            7,6,5,4,3,2,1,0,
            7,6,5,4,3,2,1,0
        };

        private uint[] _key;

        private readonly uint[] _table =
        {
            4, 10, 9, 2, 13, 8, 0, 14, 6, 11, 1, 12, 7, 15, 5, 3, 
            14, 11, 4, 12, 6, 13, 15, 10, 2, 3, 8, 1, 0, 7, 5, 9, 
            5, 8, 1, 13, 10, 3, 4, 2, 14, 15, 12, 7, 6, 0, 9, 11, 
            7, 13, 10, 1, 0, 8, 9, 15, 14, 4, 6, 12, 11, 2, 5, 3, 

            6, 12, 7, 1, 5, 15, 13, 8, 4, 10, 9, 14, 0, 3, 11, 2, 
            4, 11, 10, 0, 7, 2, 1, 13, 3, 6, 8, 5, 9, 12, 15, 14, 
            13, 11, 4, 1, 3, 15, 5, 9, 0, 14, 10, 7, 6, 8, 2, 12, 
            1, 15, 13, 0, 5, 7, 10, 4, 9, 2, 3, 14, 6, 11, 8, 12, 
        };

        public void LoadKey(
            byte[] newKey
            )
        {
            if (newKey == null)
            {
                throw new ArgumentNullException("newKey");
            }
            if (newKey.Length != 32)
            {
                throw new ArgumentException("newKey.Length != 32");
            }

            var lkey = new uint[8];

            for (int cc = 0; cc < 8; cc++)
            {
                var f0 = cc * 4 + 3;
                var f1 = cc * 4 + 2;
                var f2 = cc * 4 + 1;
                var f3 = cc * 4 + 0;

                lkey[cc] =
                        ((uint)newKey[f0]) << 24
                    | ((uint)newKey[f1]) << 16
                    | ((uint)newKey[f2]) << 8
                    | ((uint)newKey[f3])
                    ;
            }

            _key = lkey;
        }

        public byte[] EncodeBuffer(
            byte[] buffer
            )
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (buffer.Length < 1)
            {
                throw new ArgumentException("buffer.Length < 1");
            }

            var bl = buffer.Length;

            if ((bl % 8) > 0)
            {
                var cbl = bl + (8 - (bl % 8));

                var correctedBuffer = new byte[cbl];
                buffer.CopyTo(correctedBuffer, 0);

                buffer = correctedBuffer;
            }

            var cl = buffer.Length;

            var encoded = new byte[cl];

            for (var i = 0; i < cl; i += 8)
            {
                var v =
                        ((ulong)buffer[i + 0]) << 56
                    |   ((ulong)buffer[i + 1]) << 48
                    |   ((ulong)buffer[i + 2]) << 40
                    |   ((ulong)buffer[i + 3]) << 32
                    |   ((ulong)buffer[i + 4]) << 24
                    |   ((ulong)buffer[i + 5]) << 16
                    |   ((ulong)buffer[i + 6]) << 8
                    |   ((ulong)buffer[i + 7])
                    ;

                var ev = EncodeValue(v);

                //разбираем число на байты
                var evb = new byte[]
                {
                     (byte)((ev & 0xff00000000000000) >> 56),
                     (byte)((ev & 0x00ff000000000000) >> 48),
                     (byte)((ev & 0x0000ff0000000000) >> 40),
                     (byte)((ev & 0x000000ff00000000) >> 32),
                     (byte)((ev & 0x00000000ff000000) >> 24),
                     (byte)((ev & 0x0000000000ff0000) >> 16),
                     (byte)((ev & 0x000000000000ff00) >>  8),
                     (byte)((ev & 0x00000000000000ff)      )
                };

                encoded[i + 0] = evb[0];
                encoded[i + 1] = evb[1];
                encoded[i + 2] = evb[2];
                encoded[i + 3] = evb[3];
                encoded[i + 4] = evb[4];
                encoded[i + 5] = evb[5];
                encoded[i + 6] = evb[6];
                encoded[i + 7] = evb[7];
            }

            return
                encoded;
        }


        public byte[] DecodeBuffer(
            byte[] buffer
            )
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (buffer.Length < 1)
            {
                throw new ArgumentException("buffer.Length < 1");
            }

            var bl = buffer.Length;

            if ((bl%8) > 0)
            {
                var cbl = bl + (8 - (bl % 8));

                var correctedBuffer = new byte[cbl];
                buffer.CopyTo(correctedBuffer, 0);

                buffer = correctedBuffer;
            }

            var cl = buffer.Length;

            var decoded = new byte[cl];

            for (var i = 0; i < cl; i += 8)
            {
                var ev =
                        ((ulong)buffer[i + 0]) << 56
                    |   ((ulong)buffer[i + 1]) << 48
                    |   ((ulong)buffer[i + 2]) << 40
                    |   ((ulong)buffer[i + 3]) << 32
                    |   ((ulong)buffer[i + 4]) << 24
                    |   ((ulong)buffer[i + 5]) << 16
                    |   ((ulong)buffer[i + 6]) << 8
                    |   ((ulong)buffer[i + 7])
                    ;

                var dv = DecodeValue(ev);

                //разбираем число на байты
                var dvb = new byte[]
                {
                     (byte)((dv & 0xff00000000000000) >> 56),
                     (byte)((dv & 0x00ff000000000000) >> 48),
                     (byte)((dv & 0x0000ff0000000000) >> 40),
                     (byte)((dv & 0x000000ff00000000) >> 32),
                     (byte)((dv & 0x00000000ff000000) >> 24),
                     (byte)((dv & 0x0000000000ff0000) >> 16),
                     (byte)((dv & 0x000000000000ff00) >>  8),
                     (byte)((dv & 0x00000000000000ff)      )
                };

                decoded[i + 0] = dvb[0];
                decoded[i + 1] = dvb[1];
                decoded[i + 2] = dvb[2];
                decoded[i + 3] = dvb[3];
                decoded[i + 4] = dvb[4];
                decoded[i + 5] = dvb[5];
                decoded[i + 6] = dvb[6];
                decoded[i + 7] = dvb[7];
            }

            return
                decoded;
        }

        public ulong EncodeValue(ulong value)
        {
            return
                ProcessValue(value, _encodeKeyIndex);
        }

        public ulong DecodeValue(ulong value)
        {
            return
                ProcessValue(value, _decodeKeyIndex);
        }

        private ulong ProcessValue(ulong value, uint[] keyArray)
        {
            if (_key == null)
            {
                throw new InvalidOperationException("key is not loaded");
            }

            var result = value;

            //организуем 32 цикла
            for (var dd = 0; dd < 32; dd++)
            {
                //осуществляем базовый цикл

                //делим исходное число на две 32х битные части
                var n1 = (uint)(result & 0x00000000ffffffffUL);
                var n2 = (uint)((result >> 32) & 0x00000000ffffffffUL);

                //получаем текущий ключ
                var currentKey = _key[keyArray[dd]];

                //складываем с ключом
                uint ppr0 = n1 + currentKey;

                //реализуем блок подстановки

                //разбираем число на 4х битные блоки
                var sub = new uint[]
                {
                    (uint) (ppr0 & 0x0000000f) >>  0,
                    (uint) (ppr0 & 0x000000f0) >>  4,
                    (uint) (ppr0 & 0x00000f00) >>  8,
                    (uint) (ppr0 & 0x0000f000) >> 12,
                    (uint) (ppr0 & 0x000f0000) >> 16,
                    (uint) (ppr0 & 0x00f00000) >> 20,
                    (uint) (ppr0 & 0x0f000000) >> 24,
                    (uint) (ppr0 & 0xf0000000) >> 28
                };

                //заменяем данные узлами таблицы
                for (var cc = 0; cc < 8; cc++)
                {
                    sub[cc] = _table[cc * 16 + sub[cc]];
                }

                //собираем число обратно
                uint ppr1 =
                        sub[7] << 28
                    |    sub[6] << 24
                    |   sub[5] << 20
                    |   sub[4] << 16
                    |   sub[3] << 12
                    |   sub[2] << 8
                    |   sub[1] << 4
                    |   sub[0] << 0
                    ;

                //имитируем циклический сдвиг влево на 11 бит
                ulong ppr2 = ((ulong)ppr1) << 11;
                var ppr3 = (uint)(((ppr2 & 0xffffffff00000000UL) >> 32) | (ppr2 & 0x00000000ffffffffUL));

                //осуществляем побитовое сложение по модулю 2 (xor, если короче)
                uint ppr4 = ppr3 ^ n2;

                if (dd == 31)
                {
                    result = ((ulong)ppr4 << 32) | n1;
                }
                else
                {
                    result = ((ulong)n1 << 32) | ppr4;
                }
            }
            
            return
                result;
        }
    }
}
