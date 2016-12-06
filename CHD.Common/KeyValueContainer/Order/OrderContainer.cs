using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Common.Logger;

namespace CHD.Common.KeyValueContainer.Order
{
    public class OrderContainer : IOrderContainer
    {
        private const string OrderKey = "$OrderKey";

        private readonly IKeyValueContainer _container;
        private readonly IDisorderLogger _logger;

        public long Order
        {
            get
            {
                var result = 0L;

                string orderString;
                if (_container.TryGet(OrderKey, out orderString))
                {
                    long.TryParse(orderString, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
                }

                return
                    result;
            }

            set
            {
                var valueString = value.ToString(CultureInfo.InvariantCulture);

                _container.Add(OrderKey, valueString);
            }
        }

        public OrderContainer(
            IKeyValueContainer container,
            IDisorderLogger logger
            )
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _container = container;
            _logger = logger;
        }
    }
}
