using Lumos.Redis;


namespace Lumos.BLL.Biz
{

    public class RedisMq4GlobalProvider : RedisMqObject<RedisMq4GlobalHandle>
    {
        //protected override string DB_Name { get { return "Order_DBName"; } }
        protected override string MessageQueueKeyName { get { return "ReidsMq4GlobalProvider"; } }
        protected override bool IsTran { get { return false; } }


        public void Push(RedisMqHandleType handleType, object handlePms)
        {
            var obj = new RedisMq4GlobalHandle();
            obj.Type = handleType;
            obj.Pms = handlePms;
            this.Push(obj);
        }
    }
}
