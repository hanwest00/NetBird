using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetBird.Util
{
    public class Serializable
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj">需要序列化的对象</param>
        /// <returns>流</returns>
        public static byte[] SerializableToBytes(object obj)
        {
            IFormatter format = new BinaryFormatter();
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            format.Serialize(stream, obj);
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();
            return buffer;
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">反序列化后对象的类型</typeparam>
        /// <param name="buffer">流</param>
        /// <returns>对象</returns>
        public static T Deserialize<T>(System.IO.Stream buffer)
        {
            try
            {
                IFormatter format = new BinaryFormatter();
                return (T)format.Deserialize(buffer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
