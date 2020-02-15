using System.Linq;
using System.Reflection;

namespace Crypto.Algorithms.MD5Utils
{
    /// <summary>
    /// You can encryption which you want properties using this class.
    /// <para>
    /// It's important thing that you must add CryptoModelKey attribute to properties which you want to will encrypt.
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CryptoModel<T> : ICryptoModel where T : CryptoModel<T>
    {
        #region Variables
        public virtual CryptoMD5 Crypto { get; set; }

        private static PropertyInfo[] __keyProperties = null;
        #endregion

        #region Constructors
        static CryptoModel()
        {
            __keyProperties = typeof(T).GetProperties().Where(o => o.GetCustomAttributes().Any(a => a is CryptoModelKeyAttribute)).ToArray();
        }
        #endregion

        #region Methods
        private object encryptionByPropValue(object propValue)
        {
            if (propValue == null)
                return propValue;

            var type = propValue.GetType();
            if (propValue is ICryptoModel)
            {
                ((ICryptoModel)propValue).Encryption();
                return propValue;
            }
            else if (type.IsArray)
            {
                var rowIndex = 0;
                var dynamicPropValue = (dynamic)propValue;
                foreach (object item in dynamicPropValue)
                {
                    dynamicPropValue[rowIndex] = encryptionByPropValue(dynamicPropValue[rowIndex]);
                    rowIndex++;
                }

                return propValue;
            }
            else
                return this.Crypto.EncryptionAsStringToBase64(propValue.ToString());
        }

        private object decryptionByPropValue(object propValue)
        {
            if (propValue == null)
                return propValue;

            var type = propValue.GetType();
            if (propValue is ICryptoModel)
            {
                ((ICryptoModel)propValue).Decryption();
                return propValue;
            }
            else if (type.IsArray)
            {
                var rowIndex = 0;
                var dynamicPropValue = (dynamic)propValue;
                foreach (object item in dynamicPropValue)
                {
                    dynamicPropValue[rowIndex] = this.Crypto.DecryptionAsStringFromBase64(dynamicPropValue[rowIndex]);
                    rowIndex++;
                }

                return propValue;
            }
            else
                return this.Crypto.DecryptionAsStringFromBase64(propValue.ToString());
        }

        /// <summary>
        /// Model encryption
        /// </summary>
        public void Encryption()
        {
            foreach (var prop in __keyProperties)
            {
                var propValue = prop.GetValue(this);
                prop.SetValue(this, encryptionByPropValue(propValue));
            }
        }

        /// <summary>
        /// Model decryption
        /// </summary>
        public void Decryption()
        {
            foreach (var prop in __keyProperties)
            {
                var propValue = prop.GetValue(this);
                prop.SetValue(this, decryptionByPropValue(propValue));
            }
        }
        #endregion
    }
}
