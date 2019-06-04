using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1;

namespace DataBaseService
{
    public class DataBase
    {
        Writer_Reader reader { get; set; }
        InformationEntities db { get; set; }
        Table table { get; set; }
        Name name { get; set; }
        Path path { get; set; }

        public DataBase()
        {
            db = new InformationEntities();
            table = new Table();
            name = new Name();
            path = new Path();
            reader = new Writer_Reader();
        }

        public void AddToDataBase()
        {
            List<string>[] list = reader.ReadFromJson();
            List<List<string>> list2 = reader.ReadLinks();
            List<string> id = ReadIdGroupFromBD();

            IEnumerable<string> except = list[6].Except(id).ToList();

            /*var query = from id_group in list[6]
                        join a in id on id_group equals id 
                        select id_group;*/

           /* var ghg = (from a in list.Take(list.Count()) select a).ToList();

            db.Tables.AddRange(ghg[1]);*/

            if (except.Any())
            {
                for (int i = 0; i < except.Count(); i++)
                {
                    table.text = list[2][i];
                    table.likes = list[0][i];
                    table.reposts = list[1][i];
                    table.time = list[3][i];
                    table.views = list[4][i];
                    table.id_group = list[6][i];

                    //////////////////////////////////
                    
                    for (int j = 0; j < list2[i].Count; j++)
                    {
                        path.link = list2[i][j];
                        path.id_group = list[6][i];
                        path.paths = list[7][i];
                        db.Paths.Add(path);
                        db.SaveChanges();
                    }

                    //////////////////////////////////

                    name.id_group = list[6][i];
                    name.name1 = list[5][i];

                    /////////////////////////////////

                    db.Tables.Add(table);                    
                    db.Names.Add(name);
                    db.SaveChanges();
                }                           
            }
        }

        public void CleaerDataBase()
        {
            db.Tables.Remove(table);
            db.Paths.Remove(path);
            db.Names.Remove(name);
            db.SaveChanges();
        }

        public List<Table> ReadNews()
        {
            List<Table> news = (from a in db.Tables select a).ToList();
            return news;
        }

        public List<Path> ReadPictures()
        {
            List<Path> pictures = (from a in db.Paths select a).ToList();
            return pictures;
        }

        public List<Name> ReadNames()
        {
            List<Name> names = (from a in db.Names select a).ToList();
            return names;
        }

        public List<string> ReadIdGroupFromBD()
        {
            List<Name> names = ReadNames();
            List<string> id = (from a in names select a.id_group).ToList();

            return id;
        }
    }
}
