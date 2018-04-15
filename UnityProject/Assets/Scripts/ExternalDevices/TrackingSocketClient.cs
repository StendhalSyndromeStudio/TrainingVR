using UnityEngine;
using System.Collections;
using System;

namespace CVEM.Tracking
{
    public class TrackingSocketClient : MonoBehaviour, IExternalDevices
    {
        string IExternalDevices.ID
        {
            get
            {
                return this.Identifier;
            }
        }

        void IExternalDevices.IncomingMessage(string message)
        {
            try
            {
                this.Handler_IncomingMessage(message.Split(new char[] { ';' }));
            }
            catch { }
        }

        void IExternalDevices.ReStart()
        {
            this.OutcomingMessage(this, Code.Command.ToString() + ":");
        }

        void IExternalDevices.Stop()
        {

        }

        public event MainSocketClient.delOutcomingMessage OutcomingMessage = null;

        /// <summary>
        /// Описание команд
        /// </summary>
        public enum Code : int
        {
            Unknown = 0,
            Command = 1,
            Quaternion = 2,
			Magnet = 3,
		}

        /// <summary>
        /// Описание команд управления
        /// </summary>
        public enum CodeCommand : int
        {
            Unknown = 0,
            Reset = 1,
			Calibration = 2,
		}

		/// <summary>
		/// Идентификатор
		/// </summary>
		[SerializeField]
        [Header("Идентификатор")]
        private string Identifier = "CVEM";

        /// <summary>
        /// Объект управления
        /// </summary>
        [SerializeField]
        [Header("Объект управления")]
        private Transform objTransform;
        public Quaternion objectRotation
        {
            get { return objTransform.localRotation; }
        }

        private ManagerTimers managerTimers = new ManagerTimers();
        /// <summary>
        /// Таймер перезапуска
        /// </summary>
        private TimerLogic timerReset = new TimerLogic(1000) { Enabled = false };

        /// <summary>
        /// Старое значения поворота
        /// </summary>
        private Quaternion oldRotation = Quaternion.identity;
        /// <summary>
        /// Новое значение поворота
        /// </summary>
        private Quaternion newRotation = Quaternion.identity;

        /// <summary>
        /// Время между FixedUpdate
        /// </summary>
        private float fixedDeltaTime = 0f;

        [Header("Данные драйвера трекера")]
		public Quaternion outRotation;
		public UnityClientKSGT.EventValueType<bool> Magnet = new UnityClientKSGT.EventValueType<bool>(false);

		void Awake()
        {
            if (this.objTransform == null)
            {
                this.objTransform = this.transform;
            }

            this.oldRotation = this.transform.localRotation;

            this.managerTimers.Add(this.timerReset);
            this.timerReset.Tick.AddingEvent(this, this.OnTimerUpdate_Tick);
        }


        // Use this for initialization
        void Start()
        {
            MainSocketClient.AddDevice(this);
            this.timerReset.Enabled = true;
            //EventKeyBoard.Instance[KeyCode.Insert].isDown.AddingEvent(this, this.OnEventKeyBoard_R_isDown);
        }

        void Update()
        {
            this.managerTimers.CurrentTime = Time.deltaTime;
        }

        void FixedUpdate()
        {
            this.LerpQuaternion();
        }

        private void OnTimerUpdate_Tick(object Sender, object Value)
        {
            this.timerReset.Enabled = false;
            this.timerReset.Enabled = true;
            ((IExternalDevices)this).ReStart();
        }

        private void Handler_IncomingMessage(string[] commands)
        {
            foreach (string command in commands)
            {
                this.Handler_command(command);
            }
        }

        /// <summary>
        /// обработка сообщений мненджера трекинга
        /// </summary>
        /// <param name="command"></param>
        public void Handler_command(string command)
        {
            //Debug.Log( "[" + command + "]" );
            string[] array = command.Split(new char[] { ':' });
            if (array.Length != 2) return;
            Code code = UnityClientKSGT.Helper.StrToEnumDef(array[0], Code.Unknown);
            switch (code)
            {
				case Code.Magnet:
					this.Magnet.Value = UnityClientKSGT.Helper.StrToBoolDef(array[1], false);
                    break;

                case Code.Quaternion:
                    {
                        string[] str_angle = array[1].Split(new char[] { '@' });
                        float x = UnityClientKSGT.Helper.StrToFloatCultureDef(str_angle[0], 255);
                        float y = UnityClientKSGT.Helper.StrToFloatCultureDef(str_angle[1], 255);
                        float z = UnityClientKSGT.Helper.StrToFloatCultureDef(str_angle[2], 255);
                        float w = UnityClientKSGT.Helper.StrToFloatCultureDef(str_angle[3], 255);
                        if ((x == 255) || (y == 255) || (z == 255) || (w == 255)) return;
                        /*
						if (MainTrenag.useWeapon)
						{
							oldRotation = objTransform.rotation;
						}
						else
						{
                            oldRotation = objTransform.localRotation;
						}
                        */
                        oldRotation = objTransform.localRotation;
                        newRotation = new Quaternion(x, y, z, w);
						fixedDeltaTime = Mathf.Abs(Quaternion.Angle(newRotation, oldRotation) * 0.25f);
						fixedDeltaTime = Mathf.Clamp(fixedDeltaTime, 0.01f, 1f);
					}
					break;
            }
        }

        /// <summary>
        /// сглаживание вращения
        /// </summary>
        private void LerpQuaternion()
        {
			outRotation = Quaternion.Slerp(oldRotation, newRotation, fixedDeltaTime);
			if (true)
			{
				objTransform.localRotation = outRotation;
			}
		}

		/// <summary>
		/// запрос на сброс при нажатии на кнопку "Inster"
		/// </summary>
		/// <param name="Sender"></param>
		/// <param name="Value"></param>
		private void OnEventKeyBoard_R_isDown(object Sender, object Value)
        {
            if ((bool)Value)
            {
                this.OutcomingMessage(this, Code.Command.ToString() + ":" + CodeCommand.Reset.ToString());
            }
        }

        /// <summary>
        /// внешняя команда на сброс
        /// </summary>
		public void Reset()
		{
			OnEventKeyBoard_R_isDown(null, true);
		}


        public void SetNewObjectRotation(Transform newObject)
        {
            objTransform = newObject;
            //objTransform.loca
        }
    }
}