using System;
using System.Collections.Generic;
using System.Text;

namespace Base_Function.MODEL
{
    public class Class_T_Vital_Signs
    {
        private string bed_no;
        /// <summary>
        /// 床号
        /// </summary>
        public string Bed_no
        {
            get { return bed_no; }
            set { bed_no = value; }
        }
        private string pid;

        /// <summary>
        /// 病人编号
        /// </summary>
        public string Pid
        {
            get { return pid; }
            set { pid = value; }
        }

        private string measure_time;
        /// <summary>
        /// 测量时间
        /// </summary>
        public string Measure_time
        {
            get { return measure_time; }
            set { measure_time = value; }
        }
        private float temperature_value;
        /// <summary>
        /// 体温测量值
        /// </summary>
        public float Temperature_value
        {
            get { return temperature_value; }
            set { temperature_value = value; }
        }
        private string temperature_body;
        /// <summary>
        /// 体温测量部位
        /// </summary>
        public string Temperature_body
        {
            get { return temperature_body; }
            set { temperature_body = value; }
        }

        private string re_measure;
        /// <summary>
        /// 复测标志
        /// </summary>
        public string Re_measure
        {
            get { return re_measure; }
            set { re_measure = value; }
        }
        private float cooling_value;
        /// <summary>
        /// 降温后温度
        /// </summary>
        public float Cooling_value
        {
            get { return cooling_value; }
            set { cooling_value = value; }
        }

        private string cooling_type;
        /// <summary>
        /// 降温措施
        /// </summary>
        public string Cooling_type
        {
            get { return cooling_type; }
            set { cooling_type = value; }
        }

        private int pulse_value;
        /// <summary>
        /// 脉搏测量值
        /// </summary>
        public int Pulse_value
        {
            get { return pulse_value; }
            set { pulse_value = value; }
        }
        private string is_briefness;
        /// <summary>
        /// 脉搏短绌
        /// </summary>
        public string Is_briefness
        {
            get { return is_briefness; }
            set { is_briefness = value; }
        }

        private int heart_rhythm;
        /// <summary>
        /// 心率测量值
        /// </summary>
        public int Heart_rhythm
        {
            get { return heart_rhythm; }
            set { heart_rhythm = value; }
        }
        private string is_assist_hr;
        /// <summary>
        ///心率器械辅助标志
        /// </summary>
        public string Is_assist_hr
        {
            get { return is_assist_hr; }
            set { is_assist_hr = value; }
        }
        private string  breath_value;
        /// <summary>
        /// 呼吸测量值
        /// </summary>
        public string Breath_value
        {
            get { return breath_value; }
            set { breath_value = value; }
        }
        private string is_assist_br;
        /// <summary>
        /// 呼吸器械辅助标志
        /// </summary>
        public string Is_assist_br
        {
            get { return is_assist_br; }
            set { is_assist_br = value; }
        }
        private string measure_state;
        /// <summary>
        /// 测量状态
        /// </summary>
        public string Measure_state
        {
            get { return measure_state; }
            set { measure_state = value; }
        }
        private string describe;
        /// <summary>
        /// 事件描述
        /// </summary>
        public string Describe
        {
            get { return describe; }
            set { describe = value; }
        }
        private string remark;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }


        private string operater_before_time;
        /// <summary>
        /// 术前时间
        /// </summary>
        public string Operater_before_time
        {
            get { return operater_before_time; }
            set { operater_before_time = value; }
        }

        private string operater_after_time;

        /// <summary>
        /// 术后时间
        /// </summary>
        public string Operater_after_time
        {
            get { return operater_after_time; }
            set { operater_after_time = value; }
        }

        private string patient_id;
        /// <summary>
        /// 病人主键
        /// </summary>
        public string Patient_id
        {
            get { return patient_id; }
            set { patient_id = value; }
        }

        private string pain_value;
        /// <summary>
        /// 疼痛评分
        /// </summary>
        public string Pain_value
        {
            get { return pain_value; }
            set { pain_value = value; }
        }

        private string pain_mothed;
        /// <summary>
        /// 疼痛评分方法
        /// </summary>
        public string Pain_mothed
        {
            get { return pain_mothed; }
            set { pain_mothed = value; }
        }

        private string pain_value2;
        /// <summary>
        /// 疼痛评分复值
        /// </summary>
        public string Pain_value2
        {
            get { return pain_value2; }
            set { pain_value2 = value; }
        }

        private string is_qixian;
        /// <summary>
        /// 是否骑线
        /// </summary>
        public string Is_qixian
        {
            get { return is_qixian; }
            set { is_qixian = value; }
        }

        //========================骑线三测单值记录==========================================================

        private float temperature_valueQX;
        /// <summary>
        /// 体温测量值
        /// </summary>
        public float Temperature_valueQX
        {
            get { return temperature_valueQX; }
            set { temperature_valueQX = value; }
        }


        private string temperature_bodyQX;
        /// <summary>
        /// 体温测量部位
        /// </summary>
        public string Temperature_bodyQX
        {
            get { return temperature_bodyQX; }
            set { temperature_bodyQX = value; }
        }


        private string re_measureQX;
        /// <summary>
        /// 复测标志
        /// </summary>
        public string Re_measureQX
        {
            get { return re_measureQX; }
            set { re_measureQX = value; }
        }


        private float cooling_valueQX;
        /// <summary>
        /// 降温后温度
        /// </summary>
        public float Cooling_valueQX
        {
            get { return cooling_valueQX; }
            set { cooling_valueQX = value; }
        }


        private string cooling_typeQX;
        /// <summary>
        /// 降温措施
        /// </summary>
        public string Cooling_typeQX
        {
            get { return cooling_typeQX; }
            set { cooling_typeQX = value; }
        }


        private int pulse_valueQX;
        /// <summary>
        /// 脉搏测量值
        /// </summary>
        public int Pulse_valueQX
        {
            get { return pulse_valueQX; }
            set { pulse_valueQX = value; }
        }


        private string is_briefnessQX;
        /// <summary>
        /// 脉搏短绌
        /// </summary>
        public string Is_briefnessQX
        {
            get { return is_briefnessQX; }
            set { is_briefnessQX = value; }
        }


        private int heart_rhythmQX;
        /// <summary>
        /// 心率测量值
        /// </summary>
        public int Heart_rhythmQX
        {
            get { return heart_rhythmQX; }
            set { heart_rhythmQX = value; }
        }


        private string is_assist_hrQX;
        /// <summary>
        ///心率器械辅助标志
        /// </summary>
        public string Is_assist_hrQX
        {
            get { return is_assist_hrQX; }
            set { is_assist_hrQX = value; }
        }


        private string breath_valueQX;
        /// <summary>
        /// 呼吸测量值
        /// </summary>
        public string Breath_valueQX
        {
            get { return breath_valueQX; }
            set { breath_valueQX = value; }
        }


        private string is_assist_brQX;
        /// <summary>
        /// 呼吸器械辅助标志
        /// </summary> 
        public string Is_assist_brQX
        {
            get { return is_assist_brQX; }
            set { is_assist_brQX = value; }
        }
       

        private string pain_valueQX;
        /// <summary>
        /// 疼痛评分
        /// </summary>
        public string Pain_valueQX
        {
            get { return pain_valueQX; }
            set { pain_valueQX = value; }
        }

        private string pain_mothedQX;
        /// <summary>
        /// 疼痛评分方法
        /// </summary>
        public string Pain_mothedQX
        {
            get { return pain_mothedQX; }
            set { pain_mothedQX = value; }
        }

        private string pain_value2QX;
        /// <summary>
        /// 疼痛评分复值
        /// </summary>
        public string Pain_value2QX
        {
            get { return pain_value2QX; }
            set { pain_value2QX = value; }
        }
        
    }
}
