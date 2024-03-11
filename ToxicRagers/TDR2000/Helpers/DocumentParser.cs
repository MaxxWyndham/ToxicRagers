using System.ComponentModel;

using TR = ToxicRagers.Carmageddon.Helpers;

namespace ToxicRagers.TDR2000.Helpers
{
    public class DocumentParser : TR.DocumentParser
    {
        public DocumentParser(string path) : base(path) { }

        public string ReadString()
        {
            return ReadLine().Replace("\"", "");
        }

        public T Read<T>() where T : new()
        {
            T r = new();
            string line = ReadLine();
            int offset = 0;
            bool inQuote = false;

            void processProperties(object x)
            {
                var properties = x.GetType().GetProperties();

                foreach (var property in properties)
                {
                    if (!property.CanWrite || property.GetIndexParameters().Length > 0) { continue; }

                    Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    if (TypeDescriptor.GetConverter(t).CanConvertFrom(typeof(string)))
                    {
                        string v = null;
                        bool vSet = false;

                        do
                        {
                            char c = line[offset++];

                            if (c == '"')
                            {
                                inQuote = !inQuote;
                                if (!inQuote) { break; }
                            }
                            else if ((c == ' ' || c == '\t'))
                            {
                                if (!inQuote && vSet) { break; }
                            }
                            else
                            {
                                v += c;
                                vSet = true;
                            }
                        } while (offset < line.Length);

                        property.SetValue(x, Convert.ChangeType(v, t));
                    }
                    else
                    {
                        var nextLevel = property.GetValue(x, null);

                        if (nextLevel is null)
                        {
                            property.SetValue(x, Activator.CreateInstance(property.PropertyType));
                            nextLevel = property.GetValue(x, null);
                        }

                        processProperties(nextLevel);
                    }
                }
            }

            processProperties(r);

            return r;
        }
    }
}
