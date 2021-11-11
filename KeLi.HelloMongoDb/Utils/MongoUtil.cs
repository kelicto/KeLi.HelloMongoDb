using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using KeLi.HelloMongoDb.Properties;

using MongoDB;
using MongoDB.Configuration;
using MongoDB.Linq;

namespace KeLi.HelloMongoDb.Utils
{
    public class MongoUtil
    {
        public static void Insert<T>(T entity) where T: class
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            var configuration = GetConfiguration<T>();

            using (var mongo = new Mongo(configuration))
            {
                try
                {
                    mongo.Connect();

                    var database = mongo.GetDatabase(Resources.DatabaseName);
                    var collection = database.GetCollection<T>(typeof(T).Name);

                    collection.Insert(entity, true);
                    mongo.Disconnect();
                }

                catch (Exception)
                {
                    mongo.Disconnect();
                    throw;
                }
            }
        }

        public static void Delete<T>(Expression<Func<T, bool>> func) where T : class
        {
            if (func is null)
                throw new ArgumentNullException(nameof(func));

            var configuration = GetConfiguration<T>();

            using (var mongo = new Mongo(configuration))
            {
                try
                {
                    mongo.Connect();

                    var database = mongo.GetDatabase(Resources.DatabaseName);
                    var collection = database.GetCollection<T>(typeof(T).Name);

                    collection.Remove<T>(func);
                    mongo.Disconnect();
                }

                catch (Exception)
                {
                    mongo.Disconnect();
                    throw;
                }
            }
        }

        public static void Update<T>(Expression<Func<T, bool>> finder, Action<T> updater) where T : class
        {
            if (finder is null)
                throw new ArgumentNullException(nameof(finder));

            if (updater is null)
                throw new ArgumentNullException(nameof(updater));

            var configuration = GetConfiguration<T>();

            using (var mongo = new Mongo(configuration))
            {
                try
                {
                    mongo.Connect();
                    
                    var database = mongo.GetDatabase(Resources.DatabaseName);
                    var collection = database.GetCollection<T>(typeof(T).Name);
                    var entity = finder is null ? collection.Linq().FirstOrDefault() : collection.Linq().FirstOrDefault(finder);

                    updater.Invoke(entity);
                    collection.Update<T>(entity, finder, true);
                    mongo.Disconnect();
                }

                catch (Exception)
                {
                    mongo.Disconnect();
                    throw;
                }
            }
        }

        public static T Query<T>(Expression<Func<T, bool>> func = null) where T : class
        {
            if (func is null)
                throw new ArgumentNullException(nameof(func));

            var configuration = GetConfiguration<T>();

            using (var mongo = new Mongo(configuration))
            {
                try
                {
                    mongo.Connect();

                    var database = mongo.GetDatabase(Resources.DatabaseName);
                    var collection = database.GetCollection<T>(typeof(T).Name);
                    var result = func is null ? collection.Linq().FirstOrDefault() : collection.Linq().FirstOrDefault(func);

                    mongo.Disconnect();

                    return result;
                }

                catch (Exception)
                {
                    mongo.Disconnect();
                    throw;
                }
            }
        }

        public static List<T> QueryList<T>(Func<T, bool> func = null) where T : class
        {
            var configuration = GetConfiguration<T>();

            using (var mongo = new Mongo(configuration))
            {
                try
                {
                    mongo.Connect();

                    var database = mongo.GetDatabase(Resources.DatabaseName);
                    var collection = database.GetCollection<T>(typeof(T).Name);
                    var results = func is null ? collection.Linq().ToList() : collection.Linq().AsEnumerable().Where(func).ToList();

                    mongo.Disconnect();

                    return results;
                }

                catch (Exception)
                {
                    mongo.Disconnect();
                    throw;
                }
            }
        }

        private static MongoConfiguration GetConfiguration<T>() where T : class
        {
            var config = new MongoConfigurationBuilder();

            config.Mapping(mapping =>
            {
                mapping.DefaultProfile(d => d.SubClassesAre(s => s.IsSubclassOf(typeof(T))));
                mapping.Map<T>();
            });

            config.ConnectionString(Resources.ConnectionString);

            return config.BuildConfiguration();
        }
    }
}