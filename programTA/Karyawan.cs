using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace programTA
{
    class Karyawan
    {
        public string[][] AmbilDataKaryawan()
        {
            List<string> idwaj = new List<string>();
            List<string> namaMilik = new List<string>();
            try
            {
                XmlReader rea = XmlReader.Create("karyawan.xml");
                while (rea.Read())
                {
                    if (rea.IsStartElement())
                    {
                        switch (rea.Name)
                        {
                            case "id_kar":
                                if (rea.Read())
                                {
                                    idwaj.Add(rea.Value);
                                }
                                break;
                            case "nama_karyawan":
                                if (rea.Read())
                                {
                                    namaMilik.Add(rea.Value);
                                }
                                break;
                        }
                    }
                }
                rea.Close();
            }
            catch {
                XmlWriter write = XmlWriter.Create("karyawan.xml");
                write.WriteStartDocument();
                write.WriteStartElement("karyawan");
                write.WriteEndElement();
                write.WriteEndDocument();
                write.Close();
            }
            return new string[][] { idwaj.ToArray(), namaMilik.ToArray() };
        } //mengambill id wajah dan nama wajah pada database
    }
}
