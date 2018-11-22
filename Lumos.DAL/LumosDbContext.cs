using Lumos.DAL.AuthorizeRelay;
using Lumos.Entity;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Lumos.DAL
{
    public class MyEntity
    {
        public object Entity { get; set; }
        public EntityState State { get; set; }
    }
    public class LumosDbContext : AuthorizeRelayDbContext
    {

        //public FxDbContext()
        //    : base("DefaultConnection")
        //{
        //   // this.Configuration.ProxyCreationEnabled = false;
        //}

        public IDbSet<BizSn> BizSn { get; set; }
        public IDbSet<WxAutoReply> WxAutoReply { get; set; }
        public IDbSet<WxMsgPushLog> WxMsgPushLog { get; set; }
        public IDbSet<WxUserInfo> WxUserInfo { get; set; }

        public IDbSet<MchInfo> MchInfo { get; set; }
        public IDbSet<MchMenu> MchMenu { get; set; }
        public IDbSet<MchMenuPermission> MchMenuPermission { get; set; }
        public IDbSet<MchPositionMenu> MchPositionMenu { get; set; }
        public IDbSet<MchPosition> MchPosition { get; set; }
        public IDbSet<MchOrganization> MchOrganization { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public int SaveChanges(bool isSaveCache)
        {
            //判断是用重写的savechanges方法 还是普通的savechange方法
            //if (LogChangesDuringSave)
            //{
            //过滤所有修改了的实体，包括：增加 / 修改 / 删除

            var entries = from obj in this.ChangeTracker.Entries()
                          where obj.State != EntityState.Unchanged
                          select obj;

            List<MyEntity> a1 = new List<MyEntity>();

            foreach (var item in entries)
            {
                var a = new MyEntity();
                a.Entity = item.Entity;
                a.State = item.State;
                a1.Add(a);
            }


            int rows = base.SaveChanges();


            if (rows > 0)
            {
                foreach (var item in a1)
                {
                    Type entity_type = item.Entity.GetType();
                    string entity_name = item.Entity.GetType().ToString();
                    string entity_key = GetKey(item.Entity);

                    switch (item.State)
                    {
                        case EntityState.Added:
                            Console.WriteLine("Adding a {0}", item.Entity.GetType());
                            RedisHashUtil.Set(string.Format("entity:{0}", entity_name), entity_key, item.Entity);
                            //PrintPropertyValues(item.CurrentValues, item.CurrentValues.PropertyNames);
                            break;
                        case EntityState.Deleted:
                            Console.WriteLine("Deleted a {0}", item.Entity.GetType());
                            //PrintPropertyValues(item.CurrentValues, item.CurrentValues.PropertyNames);
                            break;

                        case EntityState.Modified:
                            Console.WriteLine("Modified a {0}", item.Entity.GetType());
                            RedisHashUtil.Set(string.Format("entity:{0}", entity_name), entity_key, item.Entity);
                            //PrintPropertyValues(item.CurrentValues, item.CurrentValues.PropertyNames);
                            break;
                        default:
                            break;
                    }
                    // }
                }
            }

            //返回普通的savechange方法
            return rows;
        }

        protected string GetKey(object model)
        {

            //取得m的Type实例
            Type t = model.GetType();

            string strResult = null;

            string strColumn = "Id";

            var key = model.GetType().GetProperty(strColumn).GetValue(model, null);
            if (key != null)
            {
                strResult = key.ToString();
            }

            //if (key != null)
            //{
            //    strResult = model.GetType().GetProperty(strColumn).GetValue(model, null).ToString();//直接根据属性的名字获取其值
            //}
            ////取得类的属性名并获取属性值
            foreach (System.Reflection.PropertyInfo s in t.GetProperties()) //循环遍历
            {
                if (s.Name == "Id")
                {
                    strResult = s.GetValue(model, null).ToString();
                }

            }

            return strResult;
        }
    }


    public class FxContextDatabaseInitializerForCreateDatabaseIfNotExists : CreateDatabaseIfNotExists<LumosDbContext>
    {
        protected override void Seed(LumosDbContext context)
        {
            base.Seed(context);
        }

        public List<Permission> GetPermissionList(SysPermissionCode permission)
        {
            Type t = permission.GetType();
            List<Permission> list = new List<Permission>();
            list = GetPermissionList(t, list);
            return list;
        }

        private List<Permission> GetPermissionList(Type t, List<Permission> list)
        {
            if (t.Name != "Object")
            {
                System.Reflection.FieldInfo[] properties = t.GetFields();
                foreach (System.Reflection.FieldInfo property in properties)
                {
                    string pId = "0";
                    object[] typeAttributes = property.GetCustomAttributes(false);
                    foreach (PermissionCodeAttribute attribute in typeAttributes)
                    {
                        pId = attribute.PId;
                    }
                    object id = property.GetValue(null);
                    string name = property.Name;
                    Permission model = new Permission();
                    model.Id = id.ToString();
                    model.Name = name;
                    model.PId = pId;
                    list.Add(model);
                }
                list = GetPermissionList(t.BaseType, list);
            }
            return list;
        }

    }

}
