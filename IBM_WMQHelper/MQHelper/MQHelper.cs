using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using IBM.WMQ;

namespace MQ
{
    public class MQHelper
    {
        private int _port = 1440;
        private MQQueue _queue;
        private MQQueueManager _queueManager;

        public MQHelper()
        {
            this.HostName = string.Empty;
            this._port = 5040;
            this.ChannelName = string.Empty;
            this.QueueManager = string.Empty;
            this.QueueName = string.Empty;
        }

        public void Open()
        {
            try
            {
                // mq properties
                Hashtable properties;
                properties = new Hashtable();
                properties.Add(MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED);
                properties.Add(MQC.HOST_NAME_PROPERTY, this.HostName);
                properties.Add(MQC.PORT_PROPERTY, this.Port);
                properties.Add(MQC.CHANNEL_PROPERTY, this.ChannelName);

                // create connection
                _queueManager = new MQQueueManager(this.QueueManager, properties);

                // accessing queue
                //_queue = _queueManager.AccessQueue(this.QueueName, MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING);
                _queue = _queueManager.AccessQueue(this.QueueName, MQC.MQOO_OUTPUT + MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING);
            }
            catch (MQException mqe)
            {
                throw new Exception(string.Format("MQ Error: {0}", mqe.Message));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error: {0}", ex.Message));
            }
        }

        public void Close()
        {
            try
            {

                // closing queue
                _queue.Close();

                // disconnecting queue manager
                _queueManager.Disconnect();

            }
            catch (MQException mqe)
            {
                throw new Exception(string.Format("MQ Error: {0}", mqe.Message));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error: {0}", ex.Message));
            }
        }

        public bool HasMessages()
        {
            try
            {

                if (_queue.CurrentDepth > 0)
                    return true;
                else
                    return false;

            }
            catch (MQException mqe)
            {
                throw new Exception(string.Format("MQ Error: {0}", mqe.Message));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error: {0}", ex.Message));
            }
        }

        public void PutMessage(string message)
        {
            try
            {


                // creating a message object
                MQMessage mqMessage = new MQMessage();
                mqMessage.Format = MQC.MQFMT_STRING;
                //mqMessage.CharacterSet = 437;
                //mqMessage.CharacterSet = 1208;
                mqMessage.WriteString(message);
                // putting message
                _queue.Put(mqMessage);


            }
            catch (MQException mqe)
            {
                throw new Exception(string.Format("MQ Error: {0}", mqe.Message));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error: {0}", ex.Message));
            }
        }

        public string GetMessage()
        {
            string retVal = null;
            MQMessage mqMessage = new MQMessage();
            mqMessage.Format = MQC.MQFMT_STRING;
            //mqMessage.CharacterSet = 437;
            //mqMessage.CharacterSet = 1208;
            _queue.Get(mqMessage);

            retVal = mqMessage.ReadString(mqMessage.MessageLength);
            //retVal = mqMessage.ReadString(mqMessage.DataLength);
            mqMessage.ClearMessage();
            return retVal;
        }

        #region Properties

        public string HostName { get; set; }
        public int Port { get { return _port; } set { _port = value; } }
        public string ChannelName { get; set; }
        public string QueueManager { get; set; }
        public string QueueName { get; set; }

        #endregion

        public void SetPort(string portNumber)
        {
            try
            {
                int.TryParse(portNumber, out _port);
            }
            catch
            {
                _port = 1440;
            }
        }

    }
}
