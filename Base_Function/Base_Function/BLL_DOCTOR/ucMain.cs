using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bifrost_Doctor;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Bifrost;
//using Bifrost_Doctor.CommonClass;
using TextEditor;
using DevComponents.DotNetBar;
using Bifrost_Nurse;
using System.Xml;
using Bifrost_Nurse.UControl;
using System.Collections;
//using Bifrost_Hospital_Management;

using System.IO;
using Bifrost.HisInstance;
using DevComponents.AdvTree;
using Base_Function.BASE_COMMON;
using Base_Function.BASE_DATA;
using Base_Function.MODEL;
using Base_Function.BLL_NURSE.Tempreture_Management;
using Base_Function.BLL_NURSE.Blood_pressure;
using Base_Function.BLL_NURSE.NBlood_sugarRecord;
using Base_Function.BLL_DOCTOR.NApply_Medical_Record;
using Base_Function.BLL_DOCTOR.Archive;
using Base_Function.BLL_DOCTOR.Patient_Action_Manager;
using Base_Function.BLL_DOCTOR.Consultation_Manager;
using Bifrost_Doctor.Consultation_Manager;
using Base_Function.BLL_DOCTOR.AppForm;
using Base_Function.BLL_DOCTOR.SurgeryManager;
using Base_Function.TEMPLATE;
using Base_Function.BLL_DOCTOR.Doc_Return;
using Bifrost.HisInStance;
using System.Runtime.InteropServices;
using System.Diagnostics;
using TempertureEditor;
using TempertureEditor.Tempreture_Management;
using Base_Function.BLL_DOCTOR.HisInStance.量表提醒;


namespace Base_Function.BLL_DOCTOR
{
    public partial class ucMain : UserControl
    {        
        

        ////////////////////////////////////////////////////////////////////
        //                            _ooOoo_                             //
        //                           o8888888o                            //    
        //                           88" . "88                            //    
        //                           (| ^_^ |)                            //    
        //                           O\  =  /O                            //
        //                        ____/`---'\____                         //                        
        //                      .'  \\|     |//  `.                       //
        //                     /  \\|||  :  |||//  \                      //    
        //                    /  _||||| -:- |||||-  \                     //
        //                    |   | \\\  -  /// |   |                     //
        //                    | \_|  ''\---/''  |   |                     //        
        //                    \  .-\__  `-`  ___/-. /                     //        
        //                  ___`. .'  /--.--\  `. . ___                   //    
        //                ."" '<  `.___\_<|>_/___.'  >'"".                //
        //              | | :  `- \`.;`\ _ /`;.`/ - ` : | |               //    
        //              \  \ `-.   \_ __\ /__ _/   .-` /  /               //
        //        ========`-.____`-.___\_____/___.-`____.-'========       //    
        //                             `=---='                            //
        //        ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^      //
        //         佛祖保佑            永无BUG             永不修改       //
        ////////////////////////////////////////////////////////////////////
        public Node node;

        //private ThreadStart start;
        //private Thread listenThread;
        //private static bool m_bListening = false;
        //private static System.Net.IPAddress MyIP;
        //private static TcpListener listener;
        private static Node CurrentSelectNode = null;
        private String msg;
        private delegate void RefTreeDel(NodeCollection nodes);
        private RefTreeDel dd = null;
        ISynchronizeInvoke isy;
        #region 刷新病人小卡委托
        private delegate void RefCardDel();
        #endregion
        public static Node CurrentNode = new Node();
        //保存所有的文书类型
        private AdvTree temptrvbook = new AdvTree();
        /// <summary>
        /// 当前病人对象。 
        /// </summary>
        public InPatientInfo currentPatient;
        /// <summary>
        /// 根据床号得到的病人对象
        /// </summary>
        private static InPatientInfo inpatientByBedid;
        //保存选中当前‘浏览’节点下所有文书。
        List<Patient_Doc> list_PatientDoc = new List<Patient_Doc>();
        string[] arrs = null;
        /// <summary>
        /// 术后病程记录是否有子节点
        /// </summary>
        bool isChildNode = false;
        /// <summary>
        /// 是否修改文书1修改，0未修改
        /// </summary>
        private int IsUpdate = 0;              //文书是否修改，修改1，未修改0
        /// <summary>
        /// 浏览页面修改的文书id
        /// </summary>
        private string Update_Tid = null;
        //会诊记录
        private TabControlPanel tabctpnRecord = null;
        //手术审批
        private TabControlPanel tabctpnApproval = null;
        //标志会诊记录是否加载
        private int iRecord = 0;
        //标志手术审批是否加载过
        private int iApproval = 0;
        public delegate void DeleGetRecord(string time, string content);
        private string Record_Time = null;
        private string Record_Content = null;
        //病案归档
        UcArchive ucArchive = null;
        /// <summary>
        /// 多实例文书保存成功后，返回文书id
        /// </summary>
        private string book_Id = "";
        //病案整理
        UcClear ucclear = null;
        /// <summary>
        /// 存储病人所有文书
        /// </summary>
        List<Node> lstTree = new List<Node>();
        private delegate void MethodInvokerMsg(string Msg);
        public string action_State = null;

        //public static string SectionId = "";      //寄存用户信息中的科室ID
        //public static string SectionName = "";    //寄存用户信息中的科室名称
        /// <summary>
        /// 保存提交过的文书id
        /// </summary>
        private ArrayList save_TextId = new ArrayList();
        /// <summary>
        /// 是否是定制的文书
        /// </summary>
        private bool isCustom = false;

        /// <summary>
        /// 文书是否修改
        /// </summary>
        private bool Modify = false;
        /// <summary>
        /// 上一次选中的病人的父节点文本
        /// </summary>

        string pranteText = "";

        /// <summary>
        /// 弹出时间选择窗体的返回值，点击确定返回True，点击取消返回false
        /// </summary>
        public static bool isFlagtrue = false;

        /// <summary>
        /// 授权文书操作权限，以逗号隔开
        /// </summary>
        string rightDoc_functions = "";

        /// <summary>
        /// 群录加载
        /// </summary>
        ucTempretureList ucTemperatureInfo;
        ucTempretureList ucTemperatureInfo_bb;

        public ucMain()
        {
            InitializeComponent();
         
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            arrs = DataInit.GetSids();
            DBControl.DelLog();
            try
            {
                //MyIP = System.Net.IPAddress.Parse(App.GetHostIp());
                //listener = new TcpListener(MyIP, 5567);
                //Control.CheckForIllegalCrossThreadCalls = false;
                //start = new ThreadStart(startListen);
                //listenThread = new Thread(start);
                //Thread.CurrentThread.IsBackground = true;
                //CheckForIllegalCrossThreadCalls = false;
                isy = this;
                if (App.CurrentHospitalId == 201)
                {
                    btnInArea.Visible = true;
                }
                else
                {
                    btnInArea.Visible = false;
                }

                //new System.Threading.Thread(new ThreadStart(delegate
                //{
                //if (App.UserAccount.CurrentSelectRole.Role_type == "N")
                //{
                //    if (App.UserAccount.CurrentSelectRole.Sickarea_Id !="8578992")
                //    {
                //        ucTemperatureInfo = new ucTempretureList(tempetureDataComm.TEMPLATE_NORMAL, Convert.ToInt32(App.UserAccount.CurrentSelectRole.Sickarea_Id), "_");
                //    }

                //    //if (App.UserAccount.CurrentSelectRole.Sickarea_Id == "505")
                //    //{//产科
                //    if (App.UserAccount.CurrentSelectRole.Sickarea_Id == "8578992")
                //    {
                //        ucTemperatureInfo_bb = new ucTempretureList(tempetureDataComm.TEMPLATE_CHILD, Convert.ToInt32(App.UserAccount.CurrentSelectRole.Sickarea_Id), "");
                //    }
                //    else if (App.UserAccount.CurrentSelectRole.Sickarea_Id == "8578994")
                //    {
                //        ucTemperatureInfo_bb = new ucTempretureList(tempetureDataComm.TEMPLATE_CHILD, Convert.ToInt32(App.UserAccount.CurrentSelectRole.Sickarea_Id), "_");
                //    }

                //    //}
                //}
                //}

                //    )
                //    ).Start();
            

                //XePacsInit();//PACS初始化
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //var a = ex;
            }
          
        }

        private void SetSeletTreeNode()
        {
            SetSelectNode(trvInpatientManager.Nodes, CurrentSelectNode);
        }
        /// <summary>
        /// 刷新树，并选中刷新前的节点，刷新病人小卡
        /// </summary>
        /// <param name="nodes">当前选中节点的子节点集合</param>
        /// <param name="Currentnode">当前选中的树节点</param>
        private void SetSelectNode(NodeCollection nodes, Node Currentnode)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (Currentnode != null)
                {
                    if (nodes[i].Text == Currentnode.Text)
                    {
                        trvInpatientManager.SelectedNode = nodes[i];
                        RefCardByListening();
                        return;
                    }
                    else
                    {
                        SetSelectNode(nodes[i].Nodes, Currentnode);
                    }
                }
            }
        }


        private void showmore()
        {
            App.ShowTip("信息提示", msg);
        }
        //private void showmore(string Msg)
        //{
        //    App.ShowTip("信息提示", Msg);
        //}


        DevComponents.DotNetBar.TabControlPanel tabctpnTemperatureInfo;
        DevComponents.DotNetBar.TabItem page = new DevComponents.DotNetBar.TabItem();

        DevComponents.DotNetBar.TabControlPanel tabctpnTemperatureInfo_bb;
        DevComponents.DotNetBar.TabItem page_bb = new DevComponents.DotNetBar.TabItem();

        DevComponents.DotNetBar.TabControlPanel tabctpnBloodsugarrecord;
        DevComponents.DotNetBar.TabItem page2;

        DevComponents.DotNetBar.TabControlPanel tabctpnBloodrecord;
        DevComponents.DotNetBar.TabItem page3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void saveSmallTemplate(object sender, EventArgs e)
        {
            frmSmallEdit ftmtmp = new frmSmallEdit();
            ftmtmp.ShowDialog();
        }

        /// <summary>
        /// 切换会诊界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HZ_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < this.tctlPatient.Tabs.Count; i++)
                {
                    if (this.tctlPatient.Tabs[i].Text == "会诊记录")
                    {
                        this.tctlPatient.SelectedTabIndex = i;
                        return;
                    }
                }
            }
            catch
            {

            }
        }

        private void frmhuanzhe_Load(object sender, EventArgs e)
        {
            /*
             *主界面初始化实现思路：
             * 1.当前帐号是护士，初始化病人树（待入区病人，待转入病人，科室病人，出院病人，转出病人），病人小卡，体温单群录
             * 2.当前帐号是医生，初始化病人树（我的病人，科室病人，出院病人）,病人小卡，会诊记录，手术审批，病案整理，病案审阅
             */
            //App.isOtherSystemRefrenceflag = true;
            //App.OtherSystemAccount = "cs1";
            //App.OtherSystemHisId = "1164694-1";
            try
            {                
                #region 绑定菜单按钮事件
                //查看病程
                App.A_CheckSick = null;
                App.A_CheckSick += new EventHandler(CheckSick);
                //查看体温单
                App.A_CheckTemprature = null;
                App.A_CheckTemprature += new EventHandler(CheckTemprature);
                //查看护理记录单
                App.A_tsbtnCheckNurseRecord = null;
                App.A_tsbtnCheckNurseRecord += new EventHandler(CheckNurseRecord);
                //查看检验检查结果
                App.A_CheckLis = null;
                App.A_CheckLis += new EventHandler(CheckLis);
                //查看影像报告
                App.A_CheckPacs = null;
                App.A_CheckPacs += new EventHandler(CheckPacs);
                //手术审批
                App.A_CheckOperator = null;
                App.A_CheckOperator += new EventHandler(CheckOperator);
                //病案借阅申请
                App.A_PatientSickInfoApply = null;
                App.A_PatientSickInfoApply += new EventHandler(PatientSickInfoApply);
                //归档退回申请
                App.A_BackSickInfoApply = null;
                App.A_BackSickInfoApply += new EventHandler(BackSickInfoApply);
                //运行病历查阅
                App.A_UsedSickInfoCheck = null;
                App.A_UsedSickInfoCheck += new EventHandler(UsedSickInfoCheck);
                //被授权文书操作
                App.A_DocRights = null;
                App.A_DocRights += new EventHandler(DocRights);
                //病案整理
                App.A_MedicalRecordFinishing = null;
                App.A_MedicalRecordFinishing += new EventHandler(MedicalRecordFinishing);
                //病案归档
                App.A_MedicalRecords = null;
                App.A_MedicalRecords += new EventHandler(MedicalRecords);
                //批量打印知情同意书
                App.A_BachePrint = null;
                App.A_BachePrint += new EventHandler(BachePrint);

                //保存小模版,镇平中医院:医护都开放
                App.A_SmallTemplateSave = null;
                App.A_SmallTemplateSave += new EventHandler(saveSmallTemplate);

                //未完成工作
                App.A_UnfinishedWork = null;
                App.A_UnfinishedWork += new EventHandler(UnfinishedWork);
                #endregion
                
                App.SetToolButtonByUser("ttsbtnPrint", false);
                App.SetToolButtonByUser("ttsbtnPrintContinue", false);
                App.SetToolButtonByUser("tsbtnTempSave", false);
                App.SetToolButtonByUser("tsbtnCommit", false);
               // App.SetToolButtonByUser("tsbtnSmallTemplateSave", false);
                App.SetToolButtonByUser("tsbtnTemplateSave", false);//保存模版

                if (App.UserAccount.CurrentSelectRole.Role_name.Contains("主任"))
                {
                    App.SetToolButtonByUser("tsbtnSectionAccountSets", false);
                }
                else
                {
                    App.SetToolButtonByUser("tsbtnSectionAccountSets", true);
                }

                //ds_code_type = App.GetDataSet("select * from t_data_code_type");
                if (App.UserAccount.CurrentSelectRole.Role_type == "D")     //医生站
                {
                    App.SetToolButtonByUser("tsbtnSmallTemplateSave", true);
                    App.A_BKSB += new EventHandler(BK_EVENT);//报卡上报事件绑定


                    if (App.UserAccount.CurrentSelectRole.Section_Id != "")
                    {
                        //初始化树
                        DataInit.IniTreeViewByDoctor(trvInpatientManager.Nodes);
                        trvInpatientManager.ExpandAll();
                        DevComponents.DotNetBar.TabItem page = new DevComponents.DotNetBar.TabItem();
                        page.Name = "record";
                        page.Text = "会诊记录";

                        App.UserAccount.CurrentSelectRole.Sickarea_Id = App.ReadSqlVal("select said from t_section_area where sid=" + App.UserAccount.CurrentSelectRole.Section_Id + " and rownum=1", 0, "said");
                        try
                        {
                            /*
                             *会诊记录 ----延安妇幼-会诊屏蔽
                             */
                            tabctpnRecord = new DevComponents.DotNetBar.TabControlPanel();
                            tabctpnRecord.AutoScroll = true;
                            tabctpnRecord.TabItem = page;
                            tabctpnRecord.Dock = DockStyle.Fill;
                            page.AttachedControl = tabctpnRecord;
                            this.tctlPatient.Controls.Add(tabctpnRecord);
                            this.tctlPatient.Tabs.Add(page);

                            //if (App.UserAccount.CurrentSelectRole.Role_name.Contains("科主任"))
                            //{
                            //    TabItem tabLookSign = new TabItem();
                            //    tabLookSign.Name = "tabLookSign";
                            //    tabLookSign.Text = "未签名文书";
                            //    TabControlPanel ctlpnlLookSign = new TabControlPanel();
                            //    ctlpnlLookSign.AutoScroll = true;
                            //    ctlpnlLookSign.TabItem = tabLookSign;
                            //    ctlpnlLookSign.Dock = DockStyle.Fill;
                            //    tabLookSign.AttachedControl = ctlpnlLookSign;
                            //    this.tctlPatient.Controls.Add(ctlpnlLookSign);
                            //    this.tctlPatient.Tabs.Add(tabLookSign);
                            //    tabLookSign.Click += new EventHandler(tabLookSign_Click);
                            //}
                            App.A_HZTX += new EventHandler(HZ_Click);

                        }
                        catch (Exception)
                        {
                        }

                        //App.A_SmallTemplateSave = null;
                        //App.A_SmallTemplateSave += new EventHandler(saveSmallTemplate);

                    }
                    else
                    {
                        App.Msg("科室不能为空！");
                    }
                }
                else
                {
                    if (App.UserAccount.CurrentSelectRole.Sickarea_Id != "")
                    {
                        DataInit.IniTreeView(trvInpatientManager.Nodes);
                        trvInpatientManager.ExpandAll();
                        tabctpnTemperatureInfo = new DevComponents.DotNetBar.TabControlPanel();
                        tabctpnTemperatureInfo.AutoScroll = true;
                        page = new DevComponents.DotNetBar.TabItem();
                        page.Name = "TemperatureInfo";
                        page.Text = "体温单群录";

                        tabctpnTemperatureInfo_bb = new DevComponents.DotNetBar.TabControlPanel();
                        tabctpnTemperatureInfo_bb.AutoScroll = true;
                        page_bb = new DevComponents.DotNetBar.TabItem();
                        page_bb.Name = "TemperatureInfo_bb";
                        page_bb.Text = "新生儿体温单群录";//page_bb.BackColor=wri
                                                  //tabctpnTemperatureInfo_bb.b
                        App.UserAccount.CurrentSelectRole.Section_Id = App.ReadSqlVal("select sid from t_section_area where said=" + App.UserAccount.CurrentSelectRole.Sickarea_Id + " and rownum=1", 0, "sid");
                        //tabctpnBloodsugarrecord = new DevComponents.DotNetBar.TabControlPanel();
                        //tabctpnBloodsugarrecord.AutoScroll = true;
                        //page2 = new DevComponents.DotNetBar.TabItem();
                        //page2.Name = "BloodsugarreInfo";
                        //page2.Text = "血糖单群录";

                        tabctpnBloodrecord = new DevComponents.DotNetBar.TabControlPanel();
                        tabctpnBloodrecord.AutoScroll = true;
                        page3 = new DevComponents.DotNetBar.TabItem();
                        page3.Name = "BloodInfo";
                        page3.Text = "血压群录";

                        try
                        {
                            if (App.UserAccount.CurrentSelectRole.Sickarea_Id != "8579000")
                            {
                                ucTemperatureInfo = new ucTempretureList(tempetureDataComm.TEMPLATE_NORMAL, Convert.ToInt32(App.UserAccount.CurrentSelectRole.Sickarea_Id), "_", App.UserAccount.UserInfo.User_id);
                                tabctpnTemperatureInfo.Controls.Add(ucTemperatureInfo);
                                ucTemperatureInfo.Dock = DockStyle.Fill;
                                tabctpnTemperatureInfo.TabItem = page;
                                tabctpnTemperatureInfo.Dock = DockStyle.Fill;
                                page.AttachedControl = tabctpnTemperatureInfo;
                                this.tctlPatient.Controls.Add(tabctpnTemperatureInfo);
                                this.tctlPatient.Tabs.Add(page);
                            }

                            if (App.UserAccount.CurrentSelectRole.Sickarea_Id == "8579000")
                            {
                                ucTemperatureInfo_bb = new ucTempretureList(tempetureDataComm.TEMPLATE_CHILD, Convert.ToInt32(App.UserAccount.CurrentSelectRole.Sickarea_Id), "", App.UserAccount.UserInfo.User_id);

                                tabctpnTemperatureInfo_bb.Controls.Add(ucTemperatureInfo_bb);
                                //App.UsControlStyle(ucTemperatureInfo);
                                tabctpnTemperatureInfo_bb.Dock = DockStyle.Fill;
                                tabctpnTemperatureInfo_bb.TabItem = page_bb;
                                ucTemperatureInfo_bb.Dock = DockStyle.Fill;
                                page_bb.AttachedControl = tabctpnTemperatureInfo_bb;
                                this.tctlPatient.Controls.Add(tabctpnTemperatureInfo_bb);
                                this.tctlPatient.Tabs.Add(page_bb);
                            }
                            else if (App.UserAccount.CurrentSelectRole.Sickarea_Id == "8579003")
                            {
                                ucTemperatureInfo_bb = new ucTempretureList(tempetureDataComm.TEMPLATE_CHILD, Convert.ToInt32(App.UserAccount.CurrentSelectRole.Sickarea_Id), "", App.UserAccount.UserInfo.User_id);
                                tabctpnTemperatureInfo_bb.Controls.Add(ucTemperatureInfo_bb);
                                //App.UsControlStyle(ucTemperatureInfo);
                                tabctpnTemperatureInfo_bb.Dock = DockStyle.Fill;
                                tabctpnTemperatureInfo_bb.TabItem = page_bb;
                                ucTemperatureInfo_bb.Dock = DockStyle.Fill;
                                page_bb.AttachedControl = tabctpnTemperatureInfo_bb;
                                this.tctlPatient.Controls.Add(tabctpnTemperatureInfo_bb);
                                this.tctlPatient.Tabs.Add(page_bb);
                            }

                        }
                        //this.tctlPatient.Tabs.Add(page2);
                        //this.tctlPatient.Tabs.Add(page3);
                        //}
                        catch (Exception ex)
                        {
                        }
                    }
                    else
                    {
                        App.Msg("病区不能为空！");
                    }
                }
                //DataInit.ReflashBookTree(temptrvbook);
                if (App.isOtherSystemRefrenceflag)
                {
                    if (App.OtherSystemHisId != "")
                    {
                        /*
                         * 如果是其他系统的登录调用，直接打开某个病人
                         */

                        FindSelectNode(trvInpatientManager.Nodes);
                        trvInpatientManager_DoubleClick(sender, e);
                    }
                    else
                    {
                        //默认选中病人树的‘科室病人'节点
                        for (int i = 0; i < trvInpatientManager.Nodes[0].Nodes.Count; i++)
                        {
                            if (trvInpatientManager.Nodes[0].Nodes[i].Text == "科室病人")
                            {
                                trvInpatientManager.SelectedNode = trvInpatientManager.Nodes[0].Nodes[i].Nodes[0];
                                break;
                            }
                        }
                    }
                }
                else
                {
                    /*
                     *登录成功后，默认选中病人树的‘科室病人'节点
                     */
                    for (int i = 0; i < trvInpatientManager.Nodes[0].Nodes.Count; i++)
                    {
                        if (trvInpatientManager.Nodes[0].Nodes[i].Text == "科室病人")
                        {
                            trvInpatientManager.SelectedNode = trvInpatientManager.Nodes[0].Nodes[i].Nodes[0];
                            break;
                        }
                    }
                }
                App.UsControlStyle(ucHospitalIofn1);
                
                string sql30 = @"select patient_name 姓名,(case when gender_code='0' then '男' else '女' end) 性别,age||age_unit 年龄,sick_bed_no 床位,ceil(sysdate-to_date(to_char(in_time,'yyyy-MM-dd'),'yyyy-MM-dd')) 住院天数,trunc((ceil(sysdate-to_date(to_char(in_time,'yyyy-MM-dd'),'yyyy-MM-dd'))/30)) 需写份数,(select count(*) from t_patients_doc where patient_id=a.id and textkind_id=131) 已写份数 from t_in_patient a where leave_time is null and a.document_time is null  and trunc((ceil(sysdate-to_date(to_char(in_time,'yyyy-MM-dd'),'yyyy-MM-dd'))/30))>(select count(*) from t_patients_doc where patient_id=a.id and textkind_id=131) and sick_doctor_id='" + App.UserAccount.UserInfo.User_id + "' and section_id=" + App.UserAccount.CurrentSelectRole.Section_Id + " order by cast(sick_bed_no as number)";
                //30天病人提醒
                DataSet ds = App.GetDataSet(sql30);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    Base_Function.BLL_DOCTOR.Frm30Day frm30 = new Base_Function.BLL_DOCTOR.Frm30Day(ds);
                    frm30.ShowDialog();
                }
               
            }
            catch (Exception ex)
            {

            }           
        }

        /// <summary>
        /// 寻找对应的节点并且选中
        /// 2012-10-25       
        /// </summary>
        /// <param name="nodes"></param>
        private void FindSelectNode(NodeCollection nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                InPatientInfo patient = nodes[i].Tag as InPatientInfo;
                if (patient != null && patient.His_id == App.OtherSystemHisId)
                {
                    trvInpatientManager.SelectedNode = nodes[i];
                    return;
                }
                else
                {
                    if (nodes[i].Nodes.Count > 0)
                    {
                        FindSelectNode(nodes[i].Nodes);
                    }
                }
            }
        }


        void tabLookSign_Click(object sender, EventArgs e)
        {
            TabItem tabit = sender as TabItem;
            if (tabit.AttachedControl.Controls.Count == 0)
            {
                string[] args = App.UserAccount.CurrentSelectRole.Section_name.Split('-');
                string section_name = args[args.Length - 1];
                ucLookSign ucsign = new ucLookSign(true, section_name);
                App.UsControlStyle(ucsign);
                ucsign.Dock = DockStyle.Fill;
                tabit.AttachedControl.Controls.Add(ucsign);
            }
        }

        private void treeView3_MouseDown(object sender, MouseEventArgs e)
        {
            trvInpatientManager.SelectedNode = trvInpatientManager.GetNodeAt(e.X, e.Y);
        }

        //入区
        private void 入区ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trvInpatientManager.SelectedNode != null)
            {
                InPatientInfo inpatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                string result = "";
                string pid = "";
                int patient_Id = inpatient.Id;
                InArea(ref result, ref pid);
                //LogHelper.SystemLog("", result, "入区", "", pid, patient_Id);
                //App.Msg(count.ToString());
            }
        }
        /// <summary>
        /// 入区
        /// </summary>
        /// <param name="result">入区成功标志</param>
        private void InArea(ref string result, ref string pid)
        {
            try
            {
                DataInit.isInAreaSucceed = false;    //入区操作是否成功标志     
                InPatientInfo inPatient = null;
                node = (Node)trvInpatientManager.SelectedNode.DeepCopy();
                if (node != null)
                {
                    inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                }
                frmInArea zone = new frmInArea(inPatient);
                zone.ShowDialog();
                pid = inPatient.PId;
                if (DataInit.isInAreaSucceed)        //入区操作成功后，移除当前节点，把当前节点添加到对应的科室下面，刷新树。
                {
                    result = "S";//成功
                    
                    node.Text = inPatient.Sick_Bed_Name + "  " + node.Text;
                    //foreach (TreeNode tempNode in trvInpatientManager.Nodes[0].Nodes["tnSection_patient"].Nodes)
                    //{
                    //    if (tempNode.Text== inPatient.Section_Name)
                    //    {
                    //        tempNode.Nodes.Add(node);          //添加当前节点到对应的科室
                    //    }
                    //}
                    //根据床号排列树节点的位置
                    //DataInit.RefLocationTreeNode(node, inPatient.Sick_Bed_Name, trvInpatientManager.Nodes);
                    ////刷新病人小卡
                    //UcInhospital ucInhospital =RefInpatientCard(trvInpatientManager.SelectedNode);
                    //if (ucInhospital!=null)
                    //     ucInhospital.EventRefinpatient+=new ucHospitalIofn.DelerefInpatient(ucHospitalIofn1.hospitalInfo_EventRefinpatient);
                    DataInit.RefLocationTreeNode(node, inPatient.Sick_Bed_Name, trvInpatientManager.Nodes);
                    if (DataInit.ViewSwitch == 0)
                    {
                        //刷新病人小卡
                        UCPictureBox ucPictureBox = ucPictureBoxCard(trvInpatientManager.SelectedNode, "入区");
                        //if (ucPictureBox != null)
                        //    ucPictureBox.EventReflash += new ucHospitalIofn.DelerefInpatient2(ucHospitalIofn1.pictureBox_EventReflash);
                        //RefCardByListening();         //监听器自动刷新
                    }
                    else
                    {
                        this.RefCard();
                    }
                    string name = inPatient.Patient_Name;
                    string sex = DataInit.StringFormat(inPatient.Gender_Code);
                    string bed_no = inPatient.Sick_Bed_Name;
                    string doctor_Name = inPatient.Sick_Doctor_Name;
                    string content = name + "," + sex + "," + bed_no + "," + doctor_Name + "。";
                   //App.SenderMessage(inPatient.Section_Id.ToString(), content);
                    DataInit.isInAreaSucceed = false;
                    trvInpatientManager.SelectedNode.Remove();   //移除当前节点
                }
                else
                {
                    result = "F";
                }
            }
            catch (Exception ex)
            {
                result = "F";
                App.MsgErr("入区异常信息：" + ex.Message);
            }
        }

        //重新转出
        private void 重新转出toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (trvInpatientManager.SelectedNode != null)
            {
                InPatientInfo inptient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                string result = "";
                string pid = "";
                int patient_Id = inptient.Id;
                RollOut(ref result, ref pid);
                //LogHelper.SystemLog("", result, "转出", "", pid, patient_Id);
            }
        }
        /// <summary>
        /// 重新转出
        /// </summary>
        /// <param name="result"></param>
        /// <param name="pid"></param>
        private void RollOutAgain(ref string result, ref string pid)
        {
            try
            {
                DataInit.isInAreaSucceed = false;         //转出操作是否成功
                node = (Node)trvInpatientManager.SelectedNode.DeepCopy();
                //记录选中节点
                Node tempnode = new Node();
                tempnode = trvInpatientManager.SelectedNode.Copy();
                InPatientInfo inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                frmRoll_Out inAction = new frmRoll_Out(inPatient);
                inAction.ShowDialog();

                if (DataInit.isInAreaSucceed == true)     //转出成功，移除当前节点，把当前节点添加到已转出节点下，刷新树
                {
                    result = "S";
                    node.Text = inPatient.Patient_Name;

                    DataInit.isInAreaSucceed = false;
                }
                else
                {
                    result = "F";
                }
            }
            catch (Exception ex)
            {
                result = "F";
                App.MsgErr("转出异常信息：" + ex.Message);
            }
        }

        //转出
        private void 转出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trvInpatientManager.SelectedNode != null)
            {
                InPatientInfo inptient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                string result = "";
                string pid = "";
                int patient_Id = inptient.Id;
                RollOut(ref result, ref pid);
                // LogHelper.SystemLog("", result, "转出", "", pid, patient_Id);
            }

        }
        /// <summary>
        /// 转出
        /// </summary>
        private void RollOut(ref string result, ref string pid)
        {
            try
            {
                DataInit.isInAreaSucceed = false;         //转出操作是否成功
                node = (Node)trvInpatientManager.SelectedNode.DeepCopy();
                //记录选中节点
                Node tempnode = new Node();
                tempnode = trvInpatientManager.SelectedNode.Copy();
                InPatientInfo inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                frmRoll_Out inAction = new frmRoll_Out(inPatient);
                inAction.ShowDialog();
                pid = inPatient.PId;
                if (DataInit.isInAreaSucceed == true)     //转出成功，移除当前节点，把当前节点添加到已转出节点下，刷新树
                {
                    result = "S";
                    node.Text = inPatient.Patient_Name;
                 
                    for (int i = 0; i < trvInpatientManager.Nodes[0].Nodes.Count; i++)
                    {
                        if (trvInpatientManager.Nodes[0].Nodes[i].Name == "tnYizhuanchu_patient")
                        {
                            trvInpatientManager.Nodes[0].Nodes[i].Nodes.Add(node);     //把当前节点添加到已转出节点的下面
                        }
                    }
                    //trvInpatientManager.Nodes[0].Nodes["tnYizhuanchu_patient"].Nodes.Add(node);     //把当前节点添加到已转出节点的下面
                    string name = inPatient.Patient_Name;
                    string sex = DataInit.StringFormat(inPatient.Gender_Code);
                    string doctor_Name = inPatient.Sick_Doctor_Name;
                    string content = name + "," + sex + "," + doctor_Name + "。";
                    //UcInhospital ucInhospital = RefInpatientCard(trvInpatientManager.SelectedNode);
                    //if (ucInhospital != null)
                    //    ucInhospital.EventRefinpatient += new ucHospitalIofn.DelerefInpatient(ucHospitalIofn1.hospitalInfo_EventRefinpatient);
                    //DataInit.RefLocationTreeNode(node, inPatient.Sick_Bed_Name, trvInpatientManager.Nodes);
                    if (DataInit.ViewSwitch == 0)
                    {
                        ucPictureBoxCard(tempnode, "转出");
                        ////刷新病人小卡
                        //UCPictureBox ucPictureBox = ucPictureBoxCard(trvInpatientManager.SelectedNode,"转出");
                        ////if (ucPictureBox != null)
                        ////    ucPictureBox.EventReflash += new ucHospitalIofn.DelerefInpatient2(ucHospitalIofn1.pictureBox_EventReflash);
                        //////App.Msg(content);          //发送消息
                        ////RefCardByListening();      //自动刷新树
                    }
                    else
                    {
                        this.RefCard();

                    }
                    //App.SendMessage(content, App.GetHostIp());
                    DataInit.isInAreaSucceed = false;
                    trvInpatientManager.SelectedNode.Remove();           //移除当前节点
                }
                else
                {
                    result = "F";
                }
            }
            catch (Exception ex)
            {
                result = "F";
                App.MsgErr("转出异常信息：" + ex.Message);
            }
        }
        /// <summary>
        /// 转入操作
        /// </summary>
        /// <param name="sender"></param>
        /// 
        /// <param name="e"></param>
        private void 转入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trvInpatientManager.SelectedNode != null)
            {
                string result = "";
                string pid = "";
                InPatientInfo inptient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                int patient_Id = inptient.Id;
                RollIn(ref result, ref pid);
                //LogHelper.SystemLog("", result, "转入", "", pid, patient_Id);
            }
        }

        /// <summary>
        /// 转入
        /// </summary>
        private void RollIn(ref string result, ref string pid)
        {
            try
            {

                DataInit.isInAreaSucceed = false;          //转入操作是否成功标志
                node = (Node)trvInpatientManager.SelectedNode.DeepCopy();
                InPatientInfo inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                frmTurn_In inAction = new frmTurn_In(inPatient);
                inAction.ShowDialog();
                pid = inPatient.PId;
                if (DataInit.isInAreaSucceed == true)     //转入成功后，移除当前节点，并把当前节点添加到科室病人下，刷新树
                {
                    result = "S";
                    node.Text = inPatient.Sick_Bed_Name + "  " + inPatient.Patient_Name;
                   
                    //foreach (TreeNode tempNode in trvInpatientManager.Nodes[0].Nodes["tnSection_patient"].Nodes)
                    //{
                    //    if (tempNode.Text==inPatient.Section_Name)
                    //    {
                    //        tempNode.Nodes.Add(node);          //把当前节点添加到科室病人
                    //    }
                    //}
                    string name = inPatient.Patient_Name;
                    string sex = DataInit.StringFormat(inPatient.Gender_Code);
                    string ssid = "转入" + inPatient.Sick_Area_Name + "--" + inPatient.Section_Name;
                    string content = name + "," + sex + "," + ssid + "。";
                    //刷新病人小卡
                    //UcInhospital ucInhospital = RefInpatientCard(trvInpatientManager.SelectedNode);
                    ////根据床号排列树节点的位置
                    //DataInit.RefLocationTreeNode(node, inPatient.Sick_Bed_Name, trvInpatientManager.Nodes);
                    //if (ucInhospital != null)
                    //ucInhospital.EventRefinpatient += new ucHospitalIofn.DelerefInpatient(ucHospitalIofn1.hospitalInfo_EventRefinpatient);
                    DataInit.RefLocationTreeNode(node, inPatient.Sick_Bed_Name, trvInpatientManager.Nodes);
                    if (DataInit.ViewSwitch == 0)
                    {
                        ucPictureBoxCard(trvInpatientManager.SelectedNode, "转入");
                        //刷新病人小卡
                        //UCPictureBox ucPictureBox = ucPictureBoxCard(trvInpatientManager.SelectedNode,"转入");
                        //    if (ucPictureBox != null)
                        //        ucPictureBox.EventReflash += new ucHospitalIofn.DelerefInpatient2(ucHospitalIofn1.pictureBox_EventReflash);
                        //    //App.Msg(content);                //发送消息
                        //    RefCardByListening();            //自动刷新
                    }
                    else
                    {
                        this.RefCard();
                    }
                    // App.SendMessage(content, App.GetHostIp());
                    //App.SenderMessage(inPatient.Section_Id.ToString(), content);
                    DataInit.isInAreaSucceed = false;
                    //转入成功删除病人的授权
                    string sql_Cancle = "delete from t_set_text_rights where patient_id=" + inPatient.Id;
                    App.ExecuteSQL(sql_Cancle);

                    trvInpatientManager.SelectedNode.Remove();   //移除当前节点
                }
                else
                {
                    result = "F";
                }
            }
            catch (Exception ex)
            {
                result = "F";
                App.MsgErr("转入异常信息：" + ex.Message);
            }
        }


        /// <summary>
        /// 1.常用操作TreeView树节点的选中项更改时发生
        /// 2.控制右键菜单的显示内容
        /// 3.控制刷新小卡界面的内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView3_AfterSelect(object sender, TreeViewEventArgs e)
        {


        }
        //出区
        private void 出区ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trvInpatientManager.SelectedNode != null)
            {
                InPatientInfo inpatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                int patient_Id = inpatient.Id;
                string result = "";
                string pid = "";
                OutArea(ref result, ref pid);
                //LogHelper.SystemLog("", result, "出区", "", pid, patient_Id);
            }
            else
            {
                App.Msg("请选择病人");
            }
        }

        /// <summary>
        /// 出区
        /// </summary>
        private void OutArea(ref string result, ref string pid)
        {
            try
            {
                DataInit.isInAreaSucceed = false;              //出区操作是否成功标志
                node = (Node)trvInpatientManager.SelectedNode.DeepCopy();
                InPatientInfo inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                frmOutArea outAction = new frmOutArea(inPatient);
                outAction.ShowDialog();
                pid = inPatient.PId;
                if (DataInit.isInAreaSucceed == true)         //出区成功后，移除当前节点，把当前节点添加到（已出区出院病人）节点下，刷新树
                {
                    result = "S";
                  
                    for (int i = 0; i < trvInpatientManager.Nodes[0].Nodes.Count; i++)
                    {
                        if (trvInpatientManager.Nodes[0].Nodes[i].Name == "tnYiChuyuan_patient")
                        {
                            trvInpatientManager.Nodes[0].Nodes[i].Nodes.Add(node);       //移除当前节点
                            break;
                        }
                    }
                    //trvInpatientManager.Nodes[0].Nodes["tnYiChuyuan_patient"].Nodes.Add(node);       //移除当前节点

                    string SickArea = App.UserAccount.CurrentSelectRole.Sickarea_Id;
                    if (SickArea == "")
                    {

                        SickArea = App.ReadSqlVal("select said from t_section_area t where t.sid=" + App.UserAccount.CurrentSelectRole.Sickarea_Id + " and rownum=1", 0, "said");
                    }

                    if (inPatient.Sike_Area_Id == SickArea)
                    {
                        string name = inPatient.Patient_Name;
                        string sex = DataInit.StringFormat(inPatient.Gender_Code);
                        string content = name + "," + sex + "。";
                        //UcInhospital ucInhospital = RefInpatientCard(trvInpatientManager.SelectedNode);
                        //if (ucInhospital != null)
                        //    ucInhospital.EventRefinpatient += new ucHospitalIofn.DelerefInpatient(ucHospitalIofn1.hospitalInfo_EventRefinpatient);
                        //DataInit.RefLocationTreeNode(node, inPatient.Sick_Bed_Name, trvInpatientManager.Nodes);
                        if (DataInit.ViewSwitch == 0)
                        {
                            ucPictureBoxCard(trvInpatientManager.SelectedNode, "出区");
                            //if (ucPictureBox != null)
                            //刷新病人小卡
                            //UCPictureBox ucPictureBox = ucPictureBoxCard(trvInpatientManager.SelectedNode,"出去");
                            //if (ucPictureBox != null)
                            //    ucPictureBox.EventReflash += new ucHospitalIofn.DelerefInpatient2(ucHospitalIofn1.pictureBox_EventReflash);
                            ////App.Msg(content);                    //发送消息
                            //RefCardByListening();                //自动刷新树
                        }
                        else
                        {
                            this.RefCard();
                        }

                        //App.SendMessage(content, App.GetHostIp());
                        //App.SenderMessage(inPatient.Section_Id.ToString(), content);
                        DataInit.isInAreaSucceed = false;
                        trvInpatientManager.SelectedNode.Remove();
                    }
                }
                else
                {
                    result = "F";
                }
            }
            catch (Exception ex)
            {
                result = "F";
                App.MsgErr("出区异常信息：" + ex.Message);
            }
        }
        //收回
        private void 回收ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trvInpatientManager.SelectedNode != null)
            {
                InPatientInfo inptient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                int patient_Id = inptient.Id;
                string result = "";
                string pid = "";
                //回收
                Back(ref result, ref pid);
                //LogHelper.SystemLog("", result, "回收", "", pid, patient_Id);
            }
            else
            {
                App.Msg("请选择病人！");
            }
        }

        /// <summary>
        /// 回收
        /// </summary>
        private void Back(ref string result, ref string pid)
        {
            try
            {
                DataInit.isInAreaSucceed = false;     //回收操作是否成功标志
                node = (Node)trvInpatientManager.SelectedNode.DeepCopy();
                InPatientInfo inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                frmBack_Area outAction = new frmBack_Area(inPatient);
                outAction.ShowDialog();
                pid = inPatient.PId;
                if (DataInit.isInAreaSucceed == true)       //回收成功后，移除当前节点，把当前节点添加到科室病人节点下，刷新树
                {
                    result = "S";
                    node.Text = inPatient.Sick_Bed_Name + "  " + inPatient.Patient_Name;
                    
                    //foreach (TreeNode tempNode in trvInpatientManager.Nodes[0].Nodes["tnSection_patient"].Nodes)
                    //{
                    //    if (tempNode.Text.Equals(inPatient.Section_Name))
                    //    {
                    //        tempNode.Nodes.Add(node);          //添加到对应的科室
                    //    }
                    //}
                    //根据床号排列树节点的位置
                    DataInit.RefLocationTreeNode(node, inPatient.Sick_Bed_Name, trvInpatientManager.Nodes);
                    string SickArea = App.UserAccount.CurrentSelectRole.Sickarea_Id;
                    if (SickArea == "")
                    {

                        SickArea = App.ReadSqlVal("select said from t_section_area t where t.sid=" + App.UserAccount.CurrentSelectRole.Sickarea_Id + " and rownum=1", 0, "said");
                    }

                    if (inPatient.Sike_Area_Id == SickArea)
                    {
                        if (DataInit.ViewSwitch == 0)
                        {
                            ucPictureBoxCard(trvInpatientManager.SelectedNode, "回收");

                            ////刷新病人小卡
                            //UCPictureBox ucPictureBox = ucPictureBoxCard(trvInpatientManager.SelectedNode);
                            //if (ucPictureBox != null)
                            //    ucPictureBox.EventReflash += new ucHospitalIofn.DelerefInpatient2(ucHospitalIofn1.pictureBox_EventReflash);
                            //RefCardByListening();            //自动刷新
                        }
                        else
                        {
                            this.RefCard();
                        }
                    }

                    string name = inPatient.Patient_Name;
                    string sex = DataInit.StringFormat(inPatient.Gender_Code);
                    string content = name + "," + sex + ".";
                    //App.SenderMessage(inPatient.Section_Id.ToString(), content);
                    DataInit.isInAreaSucceed = false;
                    trvInpatientManager.SelectedNode.Remove();     //移除当前节点
                }
                else
                {
                    result = "F";
                }
            }
            catch (Exception ex)
            {
                result = "F";
                App.MsgErr("回收异常" + ex.Message);
            }
        }
        //挂床
        private void 挂床ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string result = "";
            string pid = "";
            try
            {
                if (trvInpatientManager.SelectedNode != null)
                {
                    DataInit.isInAreaSucceed = false;
                    InPatientInfo inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                    frmHangBed outAction = new frmHangBed(inPatient);
                    outAction.ShowDialog();
                    if (DataInit.isInAreaSucceed == true)
                    {
                        result = "S";
                        if (inPatient.State.Equals("1"))    //挂床
                        {
                            //trvInpatientManager.SelectedNode.ForeColor = System.Drawing.Color.Red;
                            trvInpatientManager.SelectedNode.Style = elementStyleRed;
                        }
                        else
                        {
                            //trvInpatientManager.SelectedNode.ForeColor = System.Drawing.SystemColors.ControlText;
                            trvInpatientManager.SelectedNode.Style = elementStyle1;
                        }
                        string name = inPatient.Patient_Name;
                        string sex = DataInit.StringFormat(inPatient.Gender_Code);
                        string content = name + "," + sex + "。";
                        if (DataInit.ViewSwitch == 0)
                        {
                            RefCardByListening();
                        }
                        else
                        {
                            this.RefCard();
                        }

                        //App.SendMessage(content, App.GetHostIp());
                        App.SenderMessage(inPatient.Section_Id.ToString(), content);
                        DataInit.isInAreaSucceed = false;
                    }
                    else
                    {
                        result = "F";
                    }
                    int patient_Id = inPatient.Id;
                    //trvInpatientManager.SelectedNode.Tag = inPatient as object;
                    //LogHelper.SystemLog("", result, "挂床", "", pid, patient_Id);
                }
                else
                {
                    App.Msg("请选择病人！");
                }
            }
            catch (Exception ex)
            {
                result = "F";
                int patient_Id = currentPatient.Id;
                //LogHelper.SystemLog("", result, "挂床", "", pid, patient_Id);
                App.MsgErr("挂床异常" + ex.Message);
            }
        }
        /// <summary>
        /// 修改入区时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 修改入区时间ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string result = "";
            string pid = "";
            if (trvInpatientManager.SelectedNode != null)
            {
                try
                {
                    DataInit.isInAreaSucceed = false;
                    InPatientInfo inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                    frmUpdate_InArea_Time outAction = new frmUpdate_InArea_Time(inPatient);
                    outAction.ShowDialog();
                    pid = inPatient.PId;
                    if (DataInit.isInAreaSucceed)
                    {
                        result = "S";
                        string name = inPatient.Patient_Name;
                        string sex = DataInit.StringFormat(inPatient.Gender_Code);
                        string content = name + "," + sex + "。";
                        if (DataInit.ViewSwitch == 0)
                        {
                            RefCardByListening();
                        }
                        else
                        {
                            this.RefCard();
                        }
                       //App.SenderMessage(inPatient.Section_Id.ToString(), content);
                        DataInit.isInAreaSucceed = false;
                    }
                    else
                    {
                        result = "F";
                    }
                    trvInpatientManager.SelectedNode.Tag = inPatient as object;
                }
                catch (Exception ex)
                {
                    result = "F";
                    App.MsgErr("修改入区时间" + ex.Message);
                }
                //int patient_Id = currentPatient.Id;
                //LogHelper.SystemLog("", result, "修改入区时间", "", pid, patient_Id);
            }
            else
            {
                App.Msg("请选择病人！");
            }
        }

        /// <summary>
        /// 更换管床医生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 更换管床医生ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //InPatientInfo inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
            //WebReference.Service wbs = new WebReference.Service();
            //string webip = @"http://" + Encrypt.DecryptStr(App.Read_ConfigInfo("WebServerPath", "Url", Application.StartupPath + "\\Config.ini")) + @"/WebSite2/Service.asmx";
            //wbs.Url = webip;
            //DataSet ds = wbs.His_GetDataSet("select zyys from v_dzbl_zy_brry where zyh=" + inPatient.Id + "");
            //string doctor_id = ds.Tables[0].Rows[0][0].ToString();
            //if (doctor_id != inPatient.Sick_Doctor_Id)
            //{
            //    /*
            //     * 管床医生发生了变化,更换管床医生
            //     */
            //    if (App.ExecuteSQL("update t_in_patient t set t.sick_doctor_id=" + inPatient.Sick_Doctor_Id + ",t.sick_doctor_name=(select a.user_name from t_userinfo a where a.user_id=" + inPatient.Sick_Doctor_Id + ") where t.id=" + inPatient.Id + "") > 0)
            //    {
            //        App.Msg("已经同步HIS管床医生！");
            //    }
            //}
            //else
            //{
            //    App.Msg("当前病人不需要同步管床医生！");
            //}
            //string strtmp = "select * from t_in_patient where id='"+inPatient.Id+"' and die_time=null";
           
            if (trvInpatientManager.SelectedNode != null)
            {
                string pid = "";
                string result = "";
                try
                {
                    DataInit.isInAreaSucceed = false;
                    InPatientInfo inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;

                    frmUpdateDoctor outAction = new frmUpdateDoctor(inPatient);
                    outAction.ShowDialog();
                    pid = inPatient.PId;
                    if (DataInit.isInAreaSucceed == true)
                    {
                        result = "S";
                        string name = inPatient.Patient_Name;
                        string sex = DataInit.StringFormat(inPatient.Gender_Code);
                        string content = name + "," + sex + "," + inPatient.Sick_Bed_Name + "床,管床医生：" + inPatient.Sick_Doctor_Name + "。";
                        App.Msg(content);
                        if (DataInit.ViewSwitch == 0)
                        {
                            RefCardByListening();
                        }
                        else
                        {
                            this.RefCard();
                        }
                        App.SendMessage(content, App.GetHostIp());
                        DataInit.isInAreaSucceed = false;
                    }
                    else
                    {
                        result = "F";
                    }

                    trvInpatientManager.SelectedNode.Tag = inPatient as object;
                    int patient_Id = inPatient.Id;
                    //LogHelper.SystemLog("", result, "更换管床医生", "", pid, patient_Id);
                }
                catch (Exception ex)
                {
                    result = "F";
                    App.MsgErr("更换管床医生异常:" + ex.Message);
                }
            }
            else
            {
                App.Msg("请选择病人！");
            }
        }

        private void 取消转科ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trvInpatientManager.SelectedNode != null)
            {
                InPatientInfo inptient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                string result = "";
                string pid = "";
                try
                {
                    CancelRollOut(ref result, ref pid);
                }
                catch (Exception ex)
                {
                    result = "F";
                    App.MsgErr("取消转科:" + ex.Message);
                }
                int patient_Id = inptient.Id;
                //LogHelper.SystemLog("", result, "取消转科", "", pid, patient_Id);
            }
            else
            {
                App.Msg("请选择病人！");
                //App.UserAccount.Kind
                //App.UserAccount.UserInfo.Profession_card
            }
        }
        /// <summary>
        /// 取消专科
        /// </summary>
        private void CancelRollOut(ref string result, ref string pid)
        {
            DataInit.isInAreaSucceed = false;                //取消转科是否成功标志
            node = (Node)trvInpatientManager.SelectedNode.DeepCopy();
            InPatientInfo inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
            frmBackRollOut outAction = new frmBackRollOut(inPatient);
            outAction.ShowDialog();
            pid = inPatient.PId;
            Node tempNode = new Node();
            tempNode = trvInpatientManager.SelectedNode;
            if (DataInit.isInAreaSucceed == true)         //取消转科成功后，移除当前节点，添加到科室病人，刷新树
            {
                result = "S";
                node.Text = inPatient.Sick_Bed_Name + "  " + inPatient.Patient_Name;
                    //移除当前节点
                //foreach (TreeNode tempNode in trvInpatientManager.Nodes[0].Nodes["tnSection_patient"].Nodes)
                //{
                //    if (tempNode.Text.Equals(inPatient.Section_Name))
                //    {
                //        tempNode.Nodes.Add(node);            //添加到对应的科室
                //    }
                //}
                string name = inPatient.Patient_Name;
                string sex = DataInit.StringFormat(inPatient.Gender_Code);
                string content = name + "," + sex + "。";
                //根据床号排列树节点的位置
                DataInit.RefLocationTreeNode(node, inPatient.Sick_Bed_Name, trvInpatientManager.Nodes);
                if (DataInit.ViewSwitch == 0)
                {
                    ucPictureBoxCard(tempNode, "取消转科");
                    ////刷新病人小卡
                    //UCPictureBox ucPictureBox = ucPictureBoxCard(trvInpatientManager.SelectedNode);
                    //if (ucPictureBox != null)
                    //    ucPictureBox.EventReflash += new ucHospitalIofn.DelerefInpatient2(ucHospitalIofn1.pictureBox_EventReflash);
                    ////App.Msg(content);                 //发送消息
                    //RefCardByListening();             //自动刷新
                }
                else
                {
                    this.RefCard();
                }
               //App.SenderMessage(inPatient.Section_Id.ToString(), content);
                DataInit.isInAreaSucceed = false;
                trvInpatientManager.SelectedNode.Remove();
            }
            else
            {
                result = "F";
            }
        }

        private void 退回转出科室ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trvInpatientManager.SelectedNode != null)
            {
                string pid = "";
                string result = "";
                InPatientInfo inPatient = null;
                try
                {
                    DataInit.isInAreaSucceed = false;           //退回转出科室是否成功标志
                    node = (Node)trvInpatientManager.SelectedNode.DeepCopy();
                    inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                    frmCancleTurnIn outAction = new frmCancleTurnIn(inPatient);
                    outAction.ShowDialog();
                    if (DataInit.isInAreaSucceed == true)        //退回操作成功后，移除当前节点，刷新树
                    {
                        result = "S";
                   
                        string name = inPatient.Patient_Name;
                        string sex = DataInit.StringFormat(inPatient.Gender_Code);
                        string content = name + "," + sex + "。";
                        //根据床号排列树节点的位置
                        //DataInit.RefLocationTreeNode(node, inPatient.Sick_Bed_Name, trvInpatientManager.Nodes);
                        if (DataInit.ViewSwitch == 0)
                        {
                            ucPictureBoxCard(trvInpatientManager.SelectedNode, "退回转出科室");
                            ////刷新病人小卡
                            //UCPictureBox ucPictureBox = ucPictureBoxCard(trvInpatientManager.SelectedNode);
                            //if (ucPictureBox != null)
                            //    ucPictureBox.EventReflash += new ucHospitalIofn.DelerefInpatient2(ucHospitalIofn1.pictureBox_EventReflash);
                            //RefCardByListening();     //自动刷新
                        }
                        else
                        {
                            this.RefCard();
                        }
                        //App.SendMessage(content, App.GetHostIp());
                       //App.SenderMessage(inPatient.Section_Id.ToString(), content);
                        DataInit.isInAreaSucceed = false;
                        //trvInpatientManager.SelectedNode.Remove();
                    }
                    else
                    {
                        result = "F";
                    }
                    trvInpatientManager.SelectedNode.Tag = inPatient as object;
                }
                catch (Exception ex)
                {
                    result = "F";
                    App.MsgErr("退回转出科室异常：" + ex.Message);
                }
                int patient_Id = inPatient.Id;
                //LogHelper.SystemLog("", result, "退回转出科室", "", pid, patient_Id);
            }
            else
            {
                App.Msg("请选择病人！");
            }
        }

        private void 换床ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trvInpatientManager.SelectedNode != null)
            {

                DialogResult dialog = MessageBox.Show("注意：换床操作是自动同步his的,所以手动换床前,\r\n请先刷新病人界面,查看是否已经换过床！\r\n(是否继续手动操作换床？)", "注意", MessageBoxButtons.YesNo);
                if (dialog == DialogResult.Yes)
                {
                    string pid = "";
                    string result = "";
                    InPatientInfo inPatient = null;
                    try
                    {
                        if (trvInpatientManager.SelectedNode != null)
                        {
                            inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                            inPatient = DataInit.GetInpatientInfoByPid(inPatient.Id.ToString());
                        }
                        DataInit.isInAreaSucceed = false;
                        frmUpdateBed frmBed = new frmUpdateBed(inPatient);
                        frmBed.ShowDialog();
                        pid = inPatient.PId;
                        if (DataInit.isInAreaSucceed == true)
                        {

                            result = "S";
                            //TreeNode node = new TreeNode();
                            //node = trvInpatientManager.SelectedNode.Clone() as TreeNode;
                            //trvInpatientManager.SelectedNode.Remove();
                            //添加成功后，根据床号的大小，把该节点放到合适的位置
                            //DataInit.RefLocationTreeNode(node, inPatient.Sick_Bed_Name, trvInpatientManager.Nodes);
                            InPatientInfo old_Inpatient = null;
                            if (!frmBed.IsEmpty)
                            {
                                old_Inpatient = GetInpatientByTree(frmBed.Target_Bed_Id);
                            }
                            else
                            {
                                old_Inpatient = new InPatientInfo();
                                old_Inpatient.Id = 0;
                            }
                            RefAllTree();
                            //old_Inpatient.Sick_Bed_Id = inPatient.Sick_Bed_Id;
                            //old_Inpatient.Sick_Bed_Name = inPatient.Sick_Bed_Name;
                            //inPatient.Sick_Bed_Id = frmBed.Target_Bed_Id;
                            //inPatient.Sick_Bed_Name = frmBed.Target_Bed_No;
                            //刷新树
                            //ucHospitalIofn1.RefTree(trvInpatientManager.Nodes, inPatient, old_Inpatient);
                            //if (Test.ViewSwitch == 0)
                            //{
                            //    if (frmBed.IsEmpty)//如果目标床位是空的，小卡全部刷新
                            //    {
                            //        RefCard();
                            //    }
                            //    else //目标床位有人，两人互换位置
                            //    {
                            //        ucHospitalIofn1.RefCardByUpdateBed(inPatient, frmBed.Target_Bed_Id, frmBed.Target_Bed_No, ref old_Inpatient, frmBed.IsEmpty);
                            //    }
                            //    //ucHospitalIofn1.HospitalIni(DataInit.PatientsNode.Nodes, trvInpatientManager.Nodes, "fa", Test.ViewSwitch);
                            //}
                            //else
                            //{
                            this.RefCard();
                            //}
                            inPatient.Sick_Bed_Id = frmBed.Target_Bed_Id;
                            inPatient.Sick_Bed_Name = frmBed.Target_Bed_No;
                            DataInit.UpdatPatientsNodes(trvInpatientManager.SelectedNode, 4);
                            string name = inPatient.Patient_Name;
                            string sex = DataInit.StringFormat(inPatient.Gender_Code);
                            string content = name + "," + sex + "。";
                            //trvInpatientManager.SelectedNode.Text = inPatient.Sick_Bed_Name + " " + inPatient.Patient_Name;
                            //App.SenderMessage(inPatient.Section_Id.ToString(), content);
                            DataInit.isInAreaSucceed = false;
                        }
                        else
                        {
                            result = "F";
                        }
                        trvInpatientManager.SelectedNode.Tag = inPatient as object;
                    }
                    catch (Exception ex)
                    {
                        result = "F";
                        App.MsgErr("换床异常：" + ex.Message);
                    }
                    int patient_Id = inPatient.Id;
                    //LogHelper.SystemLog("", result, "换床", "", pid, patient_Id);
                }
                else
                {
                    this.RefAllTree();
                    this.RefCard();
                }
            }
            else
            {
                App.Msg("请选择病人！");
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (listenThread.ThreadState != ThreadState.Unstarted)
            //{
            //    listenThread.Suspend();
            //    listener.Stop();
            //}
            //listenThread.Abort();
            //listener.Stop();
            this.Dispose();
        }

        #region 刷新病人
        /// <summary>
        /// 刷新病人小卡
        /// </summary>
        public void RefCard()
        {
            if (DataInit.ViewSwitch == 0)   //显示病人小卡片
            {
                if (trvInpatientManager.SelectedNode == null)
                {
                    //SelectedNode.Parent
                    //App.MsgWaring("你选择的不是有效的节点！");
                    return;
                }
                if (trvInpatientManager.SelectedNode.Text == "科室病人")
                {
                    DataInit.UpdatPatientsNodes(trvInpatientManager.SelectedNode, 4);
                    ucHospitalIofn1.HospitalIni(DataInit.PatientsNode.Nodes, trvInpatientManager.Nodes, "科室病人", DataInit.ViewSwitch, trvInpatientManager);
                }
                else if (trvInpatientManager.SelectedNode.Tag == null)
                {
                    if (trvInpatientManager.SelectedNode.Parent != null)
                    {
                        if (trvInpatientManager.SelectedNode.Parent.Text == "科室病人")
                        {
                            DataInit.UpdatPatientsNodes(trvInpatientManager.SelectedNode, 4);
                            ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Nodes, trvInpatientManager.Nodes, trvInpatientManager.SelectedNode, trvInpatientManager.SelectedNode.Text, DataInit.ViewSwitch, trvInpatientManager);
                        }
                        else
                        {
                            if (
                                trvInpatientManager.SelectedNode.Text == "待入区病人" ||
                                trvInpatientManager.SelectedNode.Text.Equals("已转出病人") ||
                                trvInpatientManager.SelectedNode.Text.Equals("待转入病人") ||
                                trvInpatientManager.SelectedNode.Text.Equals("我的病人") ||
                                trvInpatientManager.SelectedNode.Text.Equals("我的出院病人"))
                            {
                                ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Nodes, trvInpatientManager.Nodes, null, trvInpatientManager.SelectedNode.Text, DataInit.ViewSwitch, trvInpatientManager);
                            }
                            else if (trvInpatientManager.SelectedNode.Text.Equals("已出院病人"))
                            {
                                DataInit.UpdatPatientsNodes(trvInpatientManager.SelectedNode, 3);
                                ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Nodes, trvInpatientManager.Nodes, null, trvInpatientManager.SelectedNode.Text, DataInit.ViewSwitch, trvInpatientManager);
                            }
                        }
                    }

                }
                else if (trvInpatientManager.SelectedNode.Tag.GetType().ToString().Contains("InPatientInfo"))
                {
                    if (trvInpatientManager.SelectedNode.Parent.Text == "待入区病人" ||
                            trvInpatientManager.SelectedNode.Parent.Text.Equals("已转出病人") ||
                            trvInpatientManager.SelectedNode.Parent.Text.Equals("待转入病人") ||
                            trvInpatientManager.SelectedNode.Parent.Text.Equals("我的病人") ||
                            trvInpatientManager.SelectedNode.Text.Equals("我的出院病人"))
                    {
                        ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Parent.Nodes, trvInpatientManager.Nodes, null, trvInpatientManager.SelectedNode.Parent.Text, DataInit.ViewSwitch, trvInpatientManager);

                    }
                    else if (trvInpatientManager.SelectedNode.Parent.Text.Equals("已出院病人"))
                    {
                        DataInit.UpdatPatientsNodes(trvInpatientManager.SelectedNode, 3);
                        ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Parent.Nodes, trvInpatientManager.Nodes, null, trvInpatientManager.SelectedNode.Parent.Text, DataInit.ViewSwitch, trvInpatientManager);
                    }
                    else
                    {
                        DataInit.UpdatPatientsNodes(trvInpatientManager.SelectedNode, 4);
                        ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Parent.Nodes, trvInpatientManager.Nodes, trvInpatientManager.SelectedNode.Parent, trvInpatientManager.SelectedNode.Parent.Text, DataInit.ViewSwitch, trvInpatientManager);
                    }
                }
            }
            else          //显示病人列表
            {
                if (trvInpatientManager.SelectedNode.Text == "科室病人")
                {
                    DataInit.UpdatPatientsNodes(trvInpatientManager.SelectedNode, 4);
                    //ucHospitalIofn1.HospitalIni(DataInit.PatientsNode.Nodes, trvInpatientManager.Nodes, "科室病人", Test.ViewSwitch);
                    ucHospitalIofn1.InpatientViewList(DataInit.PatientsNode.Nodes, true, "科室病人");
                }
                else if (trvInpatientManager.SelectedNode.Tag == null)
                {
                    if (trvInpatientManager.SelectedNode.Parent != null)
                    {
                        if (trvInpatientManager.SelectedNode.Parent.Text == "科室病人")
                        {
                            DataInit.UpdatPatientsNodes(trvInpatientManager.SelectedNode, 4);
                            ucHospitalIofn1.InpatientViewList(trvInpatientManager.SelectedNode.Nodes, true, trvInpatientManager.SelectedNode.Text);
                            // ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Nodes, trvInpatientManager.Nodes, trvInpatientManager.SelectedNode, trvInpatientManager.SelectedNode.Text, Test.ViewSwitch);
                        }
                        else
                        {
                            if (
                                trvInpatientManager.SelectedNode.Text == "待入区病人" ||
                                trvInpatientManager.SelectedNode.Text.Equals("已转出病人") ||
                                trvInpatientManager.SelectedNode.Text.Equals("待转入病人") ||
                                trvInpatientManager.SelectedNode.Text.Equals("我的病人") ||
                                trvInpatientManager.SelectedNode.Text.Equals("我的出院病人"))
                            {
                                DataInit.UpdatPatientsNodes(trvInpatientManager.SelectedNode, 4);
                                ucHospitalIofn1.InpatientViewList(trvInpatientManager.SelectedNode.Nodes, true, trvInpatientManager.SelectedNode.Text);
                                //ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Nodes, trvInpatientManager.Nodes, null, trvInpatientManager.SelectedNode.Text, Test.ViewSwitch);
                            }
                            else if (trvInpatientManager.SelectedNode.Text.Equals("已出院病人"))
                            {
                                DataInit.UpdatPatientsNodes(trvInpatientManager.SelectedNode, 3);
                                ucHospitalIofn1.InpatientViewList(trvInpatientManager.SelectedNode.Nodes, true, trvInpatientManager.SelectedNode.Text);
                                //ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Nodes, trvInpatientManager.Nodes, null, trvInpatientManager.SelectedNode.Text, Test.ViewSwitch);
                            }
                        }
                    }

                }
                else if (trvInpatientManager.SelectedNode.Tag.GetType().ToString().Contains("InPatientInfo"))
                {
                    if (trvInpatientManager.SelectedNode.Parent.Text == "待入区病人" ||
                            trvInpatientManager.SelectedNode.Parent.Text.Equals("已转出病人") ||
                            trvInpatientManager.SelectedNode.Parent.Text.Equals("待转入病人") ||
                            trvInpatientManager.SelectedNode.Parent.Text.Equals("我的病人") ||
                            trvInpatientManager.SelectedNode.Text.Equals("我的出院病人"))
                    {
                        ucHospitalIofn1.InpatientViewList(trvInpatientManager.SelectedNode.Parent.Nodes, true, trvInpatientManager.SelectedNode.Parent.Text);
                        //ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Parent.Nodes, trvInpatientManager.Nodes, null, trvInpatientManager.SelectedNode.Parent.Text, Test.ViewSwitch);

                    }
                    else if (trvInpatientManager.SelectedNode.Parent.Text.Equals("已出院病人"))
                    {
                        DataInit.UpdatPatientsNodes(trvInpatientManager.SelectedNode, 3);
                        ucHospitalIofn1.InpatientViewList(trvInpatientManager.SelectedNode.Parent.Nodes, true, trvInpatientManager.SelectedNode.Parent.Text);
                        //ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Parent.Nodes, trvInpatientManager.Nodes, null, trvInpatientManager.SelectedNode.Parent.Text, Test.ViewSwitch);
                    }
                    else
                    {
                        DataInit.UpdatPatientsNodes(trvInpatientManager.SelectedNode, 4);
                        ucHospitalIofn1.InpatientViewList(trvInpatientManager.SelectedNode.Parent.Nodes, true, trvInpatientManager.SelectedNode.Parent.Text);
                        //ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Parent.Nodes, trvInpatientManager.Nodes, trvInpatientManager.SelectedNode.Parent, trvInpatientManager.SelectedNode.Parent.Text, Test.ViewSwitch);
                    }
                }
            }

        }
        #endregion
        #region 监听器监听是否要刷新病人小卡
        /// <summary>
        /// 监听器监听是否要刷新病人小卡
        /// </summary>
        private void RefCardByListening()
        {
            if (trvInpatientManager.SelectedNode.Text == "科室病人")
            {
                DataInit.UpdatPatientsNodes(trvInpatientManager.SelectedNode, 4);
                ucHospitalIofn1.HospitalIni(DataInit.PatientsNode.Nodes, trvInpatientManager.Nodes, trvInpatientManager.SelectedNode.Text, DataInit.ViewSwitch, trvInpatientManager);
            }
            else if (trvInpatientManager.SelectedNode.Tag == null)
            {
                if (trvInpatientManager.SelectedNode.Parent.Text == "科室病人")
                {
                    //DataInit.UpdatPatientsNodes(treeView3.SelectedNode);
                    ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Nodes, trvInpatientManager.Nodes, trvInpatientManager.SelectedNode, trvInpatientManager.SelectedNode.Parent.Text, DataInit.ViewSwitch, trvInpatientManager);
                }
                else
                {
                    if (
                        trvInpatientManager.SelectedNode.Text == "待入区病人" ||
                        trvInpatientManager.SelectedNode.Text.Equals("已出院病人") ||
                        trvInpatientManager.SelectedNode.Text.Equals("已转出病人") ||
                        trvInpatientManager.SelectedNode.Text.Equals("待转入病人"))
                    {
                        ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Nodes, trvInpatientManager.Nodes, null, trvInpatientManager.SelectedNode.Text, DataInit.ViewSwitch, trvInpatientManager);
                    }
                }
            }
            else if (trvInpatientManager.SelectedNode.Tag.GetType().ToString().Contains("InPatientInfo"))
            {
                if (trvInpatientManager.SelectedNode.Parent.Text == "待入区病人" ||
                        trvInpatientManager.SelectedNode.Parent.Text.Equals("已出院病人") ||
                        trvInpatientManager.SelectedNode.Parent.Text.Equals("已转出病人") ||
                        trvInpatientManager.SelectedNode.Parent.Text.Equals("待转入病人")

                        )
                {

                    ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Parent.Nodes, trvInpatientManager.Nodes, null, trvInpatientManager.SelectedNode.Parent.Text, DataInit.ViewSwitch, trvInpatientManager);
                }
                else
                {
                    //DataInit.UpdatPatientsNodes(treeView3.SelectedNode);
                    ucHospitalIofn1.HospitalIni(trvInpatientManager.SelectedNode.Parent.Nodes, trvInpatientManager.Nodes, trvInpatientManager.SelectedNode.Parent, "科室病人", DataInit.ViewSwitch, trvInpatientManager);
                }
            }
        }
        #endregion





        /// <summary>
        /// 报卡上报事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BK_EVENT(object sender, EventArgs e)
        {
            if (DataInit.BK_ID != 0)
            {
                //验证是否已经上报
                string isBK = App.ReadSqlVal("select id from t_fecter_report_card where doc_id=" + DataInit.BK_ID, 0, "id");
                if (isBK != "" && isBK != null)
                {
                    App.Msg("该文书已经上报！");
                    return;
                }
                string textkind_id = App.ReadSqlVal("select textkind_id from t_patients_doc where tid=" + DataInit.BK_ID, 0, "textkind_id");
                int cart_type = textkind_id == "1201" ? 0 : 1;//感染报卡0传染病报卡1
                int newId = App.GenId("t_fecter_report_card", "id");
                string sql = "insert into t_fecter_report_card(ID,PATIENT_ID,CART_TYPE,STATE,SID,CREATE_ID,CREATE_TIME,DOC_ID) values(" + newId + "," + currentPatient.Id + "," + cart_type + ",0," + App.UserAccount.CurrentSelectRole.Section_Id + "," + App.UserAccount.UserInfo.User_id + ",sysdate," + DataInit.BK_ID + ")";
                int num = App.ExecuteSQL(sql);
                if (num > 0)
                {
                    App.Msg("上报成功！");
                }
            }
            else
            {
                App.MsgWaring("请先选择或提交相关的报卡文书！");
            }

        }




        private string GetTimeXml(string time)
        {
            string divtime = @"<div id='MainTell' name='MainTell' title='时间'><span>" + time + "</span><p/></div>";
            return divtime;
        }


        /// <summary>
        /// xmlnodes集合中是否含有div节点
        /// </summary>
        /// <param name="nodes">XmlNodes集合</param>
        /// <returns>true有div节点,false没有div节点</returns>
        private bool IsDiv(XmlNodeList nodes)
        {
            //是否有div节点
            bool isDiv = false;
            foreach (XmlNode node in nodes)
            {
                if (node.Name == "div")
                {
                    isDiv = true;
                    break;
                }
            }
            return isDiv;
        }

        /// <summary>
        /// 1.病人树的双击事件
        /// 2.当双击某个病人时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trvInpatientManager_DoubleClick(object sender, EventArgs e)
        {
            DataInit.boolAgree = false;
            DataInit.isRightDoc = false;
            string result = "";
            string pid = "";
            if (trvInpatientManager.SelectedNode != null)
            {
                if (trvInpatientManager.SelectedNode.Parent != null)
                {
                    if (trvInpatientManager.SelectedNode.Parent.Name == "tnYizhuanchu_patient")
                    {
                        CancelRollOut(ref result, ref pid);
                    }
                    else if (trvInpatientManager.SelectedNode.Parent.Name == "tnDairuqu")
                    {
                        InArea(ref result, ref pid);
                    }
                    else if (trvInpatientManager.SelectedNode.Parent.Name == "tnDaizhuanru")
                    {
                        RollIn(ref result, ref pid);
                    }
                    else if (trvInpatientManager.SelectedNode.Parent.Name == "tnMypatient" ||
                             (trvInpatientManager.SelectedNode.Parent.Name == "tnYiChuyuan_patient"
                        && trvInpatientManager.SelectedNode.Name != "tnMyOutPatient")
                            ||trvInpatientManager.SelectedNode.Parent.Name=="tnMyOutPatient"
                        ||(trvInpatientManager.SelectedNode.Parent.Parent != null && trvInpatientManager.SelectedNode.Parent.Parent.Name == "tnSection_patient"))
                    {
                        //验证TabControl是否有重复
                        for (int i = 0; i < tabControl_Patient.Tabs.Count; i++)
                        {
                            if (trvInpatientManager.SelectedNode.Text == tabControl_Patient.Tabs[i].Text)
                            {
                                tabControl_Patient.SelectedTabIndex = i;
                                return;
                            }
                        }
                        currentPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;

                        if (!DataInit.IsAccessRights(currentPatient.Id))
                        {
                            App.Msg("您的权限不足，请联系职能科室!");
                            return;
                        }

                        TabControlPanel tabctpnDoc = new TabControlPanel();
                        tabctpnDoc.AutoScroll = true;
                        TabItem pageDoc = new TabItem();
                        pageDoc.Name = trvInpatientManager.SelectedNode.Name;
                        pageDoc.Text = trvInpatientManager.SelectedNode.Text;
                        pageDoc.Click += new EventHandler(page_Click);
                        pageDoc.Tag = currentPatient;
                        ucDoctorOperater fm = new ucDoctorOperater(currentPatient);
                        fm.Dock = DockStyle.Fill;
                        tabctpnDoc.Controls.Add(fm);
                        tabctpnDoc.Dock = DockStyle.Fill;
                        pageDoc.AttachedControl = tabctpnDoc;
                        tabControl_Patient.Controls.Add(tabctpnDoc);
                        tabControl_Patient.Tabs.Add(pageDoc);
                        tabControl_Patient.Refresh();
                        tabControl_Patient.SelectedTab = pageDoc;
                    }
                }
            }
        }


        #region   文书操作的所有方法

        void page_Click(object sender, EventArgs e)
        {
            if (tabControl_Patient.Tabs.Count > 0)
            {
                this.tabControl_Patient.AutoCloseTabs = false;
                TabItem item = (TabItem)sender;
                //Point mp = Cursor.Position;
                MouseEventArgs mp = (MouseEventArgs)e;
                Point pTab = item.CloseButtonBounds.Location;
                if (mp.X >= pTab.X && mp.X <= pTab.X + item.CloseButtonBounds.Width && mp.Y >= pTab.Y &&
                    mp.Y <= pTab.Y + item.CloseButtonBounds.Height)
                {
                    if (App.Ask("是否关闭当前病人的文书？"))
                    {
                        App.ReleaseLockedDoc(item.Name);
                        this.tabControl_Patient.Tabs.Remove(item);
                    }
                }
            }
        }

        /// <summary>
        /// 根据选中节点的Tag的类型返回文书ID
        /// </summary>
        /// <param name="textid"></param>
        /// <returns></returns>
        private int GetTextId()
        {
            int textid = 0;
            //if (trvBookOprate.SelectedNode != null)
            //{
            //    //病人文书
            //    if (trvBookOprate.SelectedNode.Tag.GetType().ToString() == "Bifrost.Patient_Doc")
            //    {
            //        Patient_Doc doc = trvBookOprate.SelectedNode.Tag as Patient_Doc;
            //        textid = doc.Textkind_id;
            //    }
            //    //文书类型
            //    if (trvBookOprate.SelectedNode.Tag.GetType().ToString() == "Bifrost.Class_Text")
            //    {
            //        Class_Text text = trvBookOprate.SelectedNode.Tag as Class_Text;
            //        textid = text.Id;
            //    }
            //}
            return textid;
        }

        #endregion




        /// <summary>
        /// 新会诊提示
        /// </summary>
        private void btnNewConsultation_Click(object sender, EventArgs e)
        {
            frmConsultation_Remind frmRemind = new frmConsultation_Remind();
            App.FormStytleSet(frmRemind);
            frmRemind.MdiParent = App.ParentForm;
            frmRemind.Show();
        }

        private void expandableSplitter1_ExpandedChanged(object sender, ExpandedChangeEventArgs e)
        {
            //this.splitContainer1.Panel1Collapsed = false;
        }

        private void expandableSplitter1_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 刷新病人小卡
        /// </summary>
        /// <param name="node">当前选中的节点</param>
        /// <param name="controls">对应的病人卡片</param>
        /// <returns>返回病人卡片</returns>
        private UcInhospital RefInpatientCard(TreeNode node)
        {
            UcInhospital uchospital_Result = null;
            //选中当期节点的病人pid
            string CurrentId = node.Name;
            foreach (Control control in ucHospitalIofn1.flowLayoutPanel1.Controls)
            {
                UcInhospital uchospital = control as UcInhospital;
                if (CurrentId == uchospital.Name)  //如果当前节点的pid和病人小卡的pid相同
                {
                    uchospital_Result = uchospital;
                }
            }
            return uchospital_Result;
        }

        /// <summary>
        /// 刷新病人小卡
        /// </summary>
        /// <param name="node">当前选中的节点</param>
        /// <param name="controls">对应的病人卡片</param>
        /// /// <param name="oprate_Name">操作类型</param>
        /// <returns>返回病人卡片</returns>
        private UCPictureBox ucPictureBoxCard(Node node, string oprate_Name)
        {
            UCPictureBox uchospital_Result = null;
            //选中当期节点的病人pid
            string CurrentId = node.Name;
            foreach (Control control in ucHospitalIofn1.flowLayoutPanel1.Controls)
            {
                UCPictureBox uchospital = control as UCPictureBox;
                if (CurrentId == uchospital.Name)  //如果当前节点的pid和病人小卡的pid相同
                {
                    if (oprate_Name == "入区" || oprate_Name == "转入" ||
                        oprate_Name == "退回转出科室" || oprate_Name == "回收" ||
                        oprate_Name == "取消转科")
                    {
                        ucHospitalIofn1.pictureBox_EventRefinpatient(uchospital);
                        break;
                    }
                    else if (oprate_Name == "转出" || oprate_Name == "出区")
                    {

                        InPatientInfo inptient = new InPatientInfo();
                        inptient.Id = 0;
                        inptient.Sick_Bed_Id = uchospital.Inpat.Sick_Bed_Id;
                        inptient.Sick_Bed_Name = uchospital.Inpat.Sick_Bed_Name;
                        uchospital.Img(inptient);
                        uchospital.Inpat = inptient;
                    }
                    else
                    {
                        uchospital_Result = uchospital;
                    }
                }
            }
            return uchospital_Result;
        }

        private void tctlDoc_ControlRemoved(object sender, ControlEventArgs e)
        {
            //if (tctlDoc.SelectedTab != null)
            //{
            //    if (!tctlDoc.SelectedTab.Text.Contains("浏览"))
            //    {
            //        if (tctlDoc.SelectedTab.Tag != null)
            //        {
            //            InPatientInfo tempinfo = (InPatientInfo)tctlDoc.SelectedTab.Tag;
            //            App.DelCurrentDocMsg(tempinfo.Id + tctlDoc.SelectedTab.Text);
            //        }
            //    }
            //}

        }

        private void tctlDoc_TabRemoved(object sender, EventArgs e)
        {
            App.Ask("您还没有提交，是否真的要关闭？");
            //string text = tctlDoc.SelectedTab.Text;
            //bool isCommit = IsCommit(tctlDoc.SelectedTab.Name);
            //if (!isCommit)
            //{
            //    if (App.Ask("您还没有提交，是否真的要关闭？"))
            //    {
            //        tctlDoc.SelectedTab.Dispose();
            //        tctlDoc.SelectedPanel.Dispose();
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}
        }

        private void 会诊申请ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                InPatientInfo inpat = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                frmConsultation_Apply frmconsultation_apply = new frmConsultation_Apply(inpat);
                App.FormStytleSet(frmconsultation_apply, false);
                //App.AddNewChildForm(frmconsultation_apply);
                frmconsultation_apply.ShowDialog();
            }
            catch (Exception ex)
            {

                App.MsgErr("会诊申请异常信息：" + ex.Message);
            }
        }

        private void 病人基本信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                DataInit.isInAreaSucceed = false;
                InPatientInfo Inpatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                frmPatientInfo frmpatient = new frmPatientInfo(Inpatient);
                frmpatient.StartPosition = FormStartPosition.CenterParent;
                frmpatient.ShowDialog();
                if (DataInit.isInAreaSucceed)
                {
                    string name = Inpatient.Patient_Name;
                    string sex = DataInit.StringFormat(Inpatient.Gender_Code);
                    string bed_no = Inpatient.Sick_Bed_Name;
                    string doctor_Name = Inpatient.Sick_Doctor_Name;
                    string content = name + "," + sex + "," + bed_no + "," + doctor_Name + "。";
                    //App.Msg(content);
                    App.SendMessage(content, App.GetHostIp());
                    DataInit.UpdatPatientsNodes(trvInpatientManager.SelectedNode, 4);
                    if (DataInit.ViewSwitch == 0) //病人小卡显示
                    {
                        //ucHospitalIofn1.HospitalIni(DataInit.PatientsNode.Nodes, trvInpatientManager.Nodes, "科室病人", Test.ViewSwitch,trvInpatientManager);
                        UCPictureBox pictureBox = null;
                        for (int i = 0; i < ucHospitalIofn1.flowLayoutPanel1.Controls.Count; i++)
                        {
                            InPatientInfo picPatient = ucHospitalIofn1.flowLayoutPanel1.Controls[i].Tag as InPatientInfo;
                            if (picPatient != null && picPatient.Id == currentPatient.Id)
                            {
                                pictureBox = ucHospitalIofn1.flowLayoutPanel1.Controls[i] as UCPictureBox;
                                break;
                            }
                        }
                        UCPictureBox.dianose_Name = DataInit.GetDiagnose(Inpatient.Id.ToString());
                        pictureBox.Img(Inpatient);
                    }
                    else
                    {
                        this.RefCard();   //病人列表显示
                    }
                    DataInit.isInAreaSucceed = false;
                }
            }
            catch (Exception ex)
            {

                App.MsgErr("病人基本信息异常信息：" + ex.Message);
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            /*
             * 实时监听出入转操作，刷新树，病人小卡，发送消息
             */
            try
            {

                string content = App.getMesContent(arrs);
                if (content != "")
                {
                    msg = content;
                    if (trvInpatientManager != null)
                    {
                        if (!App.IsReceiceMsg(content))
                        {
                            if (App.UserAccount.CurrentSelectRole.Role_type == "D")
                            {
                                dd = new RefTreeDel(DataInit.IniTreeViewByDoctor);
                            }
                            else
                            {
                                dd = new RefTreeDel(DataInit.IniTreeView);
                            }
                            trvInpatientManager.Invoke(dd, trvInpatientManager.Nodes);
                            trvInpatientManager.ExpandAll();
                            MethodInvoker tinvo = new MethodInvoker(showmore);
                            isy.Invoke(tinvo, null);
                            MethodInvoker tinvo2 = new MethodInvoker(SetSeletTreeNode);
                            isy.Invoke(tinvo2, null);
                            App.AddTempUserMsg(content);
                        }
                    }
                }
            }
            catch//(Exception ex) 
            {
                //App.MsgErr("监听器异常信息：" + ex.Message);
            }
        }

        private void tctlPatient_SelectedTabChanged(object sender, TabStripTabChangedEventArgs e)
        {
            if (App.UserAccount.CurrentSelectRole.Role_type == "D")
            {
                if (tctlPatient.SelectedTabIndex == 1 && iRecord == 0)   //会诊
                {
                    try
                    {
                        if (tabctpnRecord.Controls.Count == 0)
                        {
                            ucConsultation_record frmRecordNew = new ucConsultation_record();
                            frmRecordNew.browse_Book += new ucConsultation_record.RefEventHandler(ucapply_browse_Book2);
                            frmRecordNew.Dock = DockStyle.Fill;
                            tabctpnRecord.Controls.Add(frmRecordNew);
                            App.UsControlStyle(frmRecordNew);
                            iRecord++;
                        }
                    }
                    catch (Exception ex)
                    {
                        App.MsgErr("会诊异常信息：" + ex.Message);
                    }
                }
                //else if (tctlPatient.SelectedTabIndex == 2 && iApproval == 0)  //手术审批
                //{
                //    try
                //    {
                //        if (tabctpnApproval.Controls.Count == 0)
                //        {
                //            UcApproval ucApproval = new UcApproval();
                //            ucApproval.Dock = DockStyle.Fill;
                //            tabctpnApproval.Controls.Add(ucApproval);
                //            App.UsControlStyle(ucApproval);
                //            iApproval++;
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        App.MsgErr("手术审批异常信息：" + ex.Message);
                //    }
                //}
            }
            else if (App.UserAccount.CurrentSelectRole.Role_type == "N")
            {
                //if (tctlPatient.SelectedTab.Name == "TemperatureInfo")
                //{
                //    App.SetMainFrmMsgToolBarText("");
                //    /*
                //     * 体温单群录
                //     */
                //    if (tabctpnTemperatureInfo.Controls.Count == 0)
                //    {
                //        if (ucTemperatureInfo == null)
                //        {
                //            ucTemperatureInfo = new ucTempretureList(tempetureDataComm.TEMPLATE_NORMAL, Convert.ToInt32(App.UserAccount.CurrentSelectRole.Sickarea_Id),"_");
                //        }
                //        tabctpnTemperatureInfo.Controls.Add(ucTemperatureInfo);
                        
                //        ucTemperatureInfo.Dock = DockStyle.Fill;
                //        tabctpnTemperatureInfo.TabItem = page;
                //        tabctpnTemperatureInfo.Dock = DockStyle.Fill;
                //        page.AttachedControl = tabctpnTemperatureInfo;
                //        this.tctlPatient.Controls.Add(tabctpnTemperatureInfo);
                //    }
                //    else
                //    {
                //        try
                //        {
                //            //ucTempretureList ucTemperatureInfo = tabctpnTemperatureInfo.Controls[0] as ucTempretureList;
                //            //ucTemperatureInfo.ShowDate();
                //        }
                //        catch { }
                //    }
                //}
                //else if (tctlPatient.SelectedTab.Name == "TemperatureInfo_bb")
                //{
                //    App.SetMainFrmMsgToolBarText("");
                //    /*
                //     * 新生儿体温单群录
                //     */
                //    if (tabctpnTemperatureInfo_bb.Controls.Count == 0)
                //    {
                //        if (ucTemperatureInfo_bb == null)
                //        {
                //            if (App.UserAccount.CurrentSelectRole.Sickarea_Id == "8579000")
                //            {
                //                ucTemperatureInfo_bb = new ucTempretureList(tempetureDataComm.TEMPLATE_CHILD, Convert.ToInt32(App.UserAccount.CurrentSelectRole.Sickarea_Id), "");
                //            }
                //            else
                //            {
                //                ucTemperatureInfo_bb = new ucTempretureList(tempetureDataComm.TEMPLATE_CHILD, Convert.ToInt32(App.UserAccount.CurrentSelectRole.Sickarea_Id), "_");
                //            }
                //        }
                //        tabctpnTemperatureInfo_bb.Controls.Add(ucTemperatureInfo_bb);
                //        App.UsControlStyle(ucTemperatureInfo);
                //        //App.UsControlStyle(ucTemperatureInfo);
                //        tabctpnTemperatureInfo_bb.Dock = DockStyle.Fill;
                //        tabctpnTemperatureInfo_bb.TabItem = page_bb;
                //        ucTemperatureInfo_bb.Dock = DockStyle.Fill;
                //        page_bb.AttachedControl = tabctpnTemperatureInfo_bb;
                //        this.tctlPatient.Controls.Add(tabctpnTemperatureInfo_bb);

                //    }
                //    else
                //    {
                //        try
                //        {
                //            //ucTempretureList ucTemperatureInfo = tabctpnTemperatureInfo.Controls[0] as ucTempretureList;
                //            //ucTemperatureInfo.ShowDate();
                //        }
                //        catch { }
                //    }
                //}
                //else 
                if (tctlPatient.SelectedTab.Name == "BloodsugarreInfo")
                {
                    App.SetMainFrmMsgToolBarText("");
                    /*
                     * 血糖单群录
                     */
                    if (tabctpnBloodsugarrecord.Controls.Count == 0)
                    {
                        UcBlood_SugarRecord ucbloodsugarrecord = new UcBlood_SugarRecord();
                        tabctpnBloodsugarrecord.Controls.Add(ucbloodsugarrecord);
                        App.UsControlStyle(ucbloodsugarrecord);
                        tabctpnBloodsugarrecord.TabItem = page2;
                        tabctpnBloodsugarrecord.Dock = DockStyle.Fill;
                        page2.AttachedControl = tabctpnBloodsugarrecord;
                        this.tctlPatient.Controls.Add(tabctpnBloodsugarrecord);
                        //this.tctlPatient.Tabs.Add(page2);
                        ucbloodsugarrecord.Dock = DockStyle.Fill;
                    }
                }
                else if (tctlPatient.SelectedTab.Name == "BloodInfo")
                {
                    App.SetMainFrmMsgToolBarText("");
                    if (tabctpnBloodrecord.Controls.Count == 0)
                    {
                        /*
                         * 血压群录
                         */
                        UcBlood_Pressure ucbloodrecord = new UcBlood_Pressure();
                        ucbloodrecord.Dock = DockStyle.Fill;
                        tabctpnBloodrecord.Controls.Add(ucbloodrecord);
                        App.UsControlStyle(ucbloodrecord);
                        tabctpnBloodrecord.TabItem = page3;
                        tabctpnBloodrecord.Dock = DockStyle.Fill;
                        page3.AttachedControl = tabctpnBloodrecord;
                        this.tctlPatient.Controls.Add(tabctpnBloodrecord);
                        //this.tctlPatient.Tabs.Add(page3);
                        tabctpnBloodrecord.Dock = DockStyle.Fill;
                    }
                }

            }
            else
            {
                if (tctlPatient.SelectedTab.Name == "UcClear")   //病案整理
                {
                    App.SetMainFrmMsgToolBarText("");
                    ucclear.DataBind();
                }
                else if (tctlPatient.SelectedTab.Name == "UcArchive")   //病案归档
                {
                    App.SetMainFrmMsgToolBarText("");
                    ucArchive.DataBind();
                }
                else if (tctlPatient.SelectedTab.Name.Contains("TemperatureInfo"))
                {
                    App.SetMainFrmMsgToolBarText("");
                }

            }
        }
        //手术
        private void 手术ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                InPatientInfo inpat = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                frmSurgery frm = new frmSurgery(inpat);
                //App.AddNewChildForm(frm);
            }
            catch (Exception ex)
            {

                App.MsgErr("手术操作异常信息：" + ex.Message);
            }
        }


        /// <summary>
        /// 获得文书记录时间，记录内容
        /// </summary>
        /// <param name="time">记录时间</param>
        /// <param name="content">记录内容</param>
        private void GetDate(string time, string content)
        {
            this.Record_Time = time;
            this.Record_Content = content;
        }

        //切换视图
        private void btnSwitch_Click(object sender, EventArgs e)
        {
            ucHospitalIofn1.flowLayoutPanel1.Controls.Clear();
            if (DataInit.ViewSwitch == 0) //当前为小卡显示
            {
                DataInit.ViewSwitch = 1;
                tabitPatinetCard.Text = "病人列表";
            }
            else
            {
                DataInit.ViewSwitch = 0;
                tabitPatinetCard.Text = "病人小卡";
            }
            try
            {
                RefCard();
            }
            catch (Exception ex)
            {

                App.MsgErr("视图切换：" + ex.Message);
            }
        }
        /// <summary>
        /// 获得文书的类型
        /// </summary>
        /// <returns></returns>
        private List<ListItem> GetListBookType()
        {
            List<ListItem> list = new List<ListItem>();
            string sql = "select b.* from t_data_code t" +
                         " inner join t_text b on t.id = b.isbelongtotype" +
                         " where t.type='31' and b.parentid=0 and b.enable_flag='Y'";
            DataSet ds = App.GetDataSet(sql);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                ListItem Dlist = new ListItem("0", "--所有文书类型--", "");
                list.Add(Dlist);
                foreach (DataRow row in dt.Rows)
                {
                    ListItem item = new ListItem();
                    item.Id = row["id"].ToString();
                    item.Name = row["Textname"].ToString();
                    item.Sid = row["isbelongtotype"].ToString();
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// 修改文书
        /// </summary>
        /// <param name="tid"></param>
        private void Rethreee_CreateTab(string tid)
        {
            //if (tid != "")
            //{
            //    SelectedNodeByTid(trvBookOprate.Nodes, tid);
            //    if (!IsSameTabItem(tid))
            //    {
            //        CreateTabItem(Convert.ToInt32(tid));
            //    }
            //}
        }
        private void SelectedNodeByTid(NodeCollection nodes, string tid)
        {
            foreach (Node node in nodes)
            {
                if (node.Name == tid)
                {
                    //trvBookOprate.SelectedNode = node;
                    break;
                }
                if (node.Nodes.Count > 0)
                {
                    SelectedNodeByTid(node.Nodes, tid);
                }
            }
        }

        private void 历史病历查看ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (trvBookOprate.SelectedNode != null)
            //{
            //    InPatientInfo inpatient = trvBookOprate.SelectedNode.Tag as InPatientInfo;
            //    frmBookHistory frmbookHistory = new frmBookHistory(inpatient);
            //    frmbookHistory.ShowDialog();
            //}
        }

        //排序问题
        private static void restorrecords(ref string[] strtitles)
        {
            try
            {
                for (int i = 1; i < strtitles.Length / strtitles.Rank; i++)
                {

                    if (strtitles[i - 1] != null)
                    {
                        //if (strtitles[ i - 1] == strtitles[i])//判断是否是相同的文书id
                        //{
                        DateTime dt1;
                        DateTime dt2;
                        dt1 = Convert.ToDateTime(App.GetTimeString(strtitles[i - 1].TrimStart()));
                        dt2 = Convert.ToDateTime(App.GetTimeString(strtitles[i].TrimStart()));
                        if (dt1 > dt2)
                        {
                            string temp = strtitles[i - 1];
                            strtitles[i - 1] = strtitles[i];
                            strtitles[i] = temp;
                            restorrecords(ref strtitles);
                        }
                        //}
                    }
                }
            }
            catch (Exception e)
            { }
        }




        private void tlspmnitReflash_Click(object sender, EventArgs e)
        {
            RefAllTree();
            RefCard();
        }
        /// <summary>
        /// 刷新树
        /// </summary>
        public void RefAllTree()
        {
            Node selectNode = trvInpatientManager.SelectedNode;
            if (App.UserAccount.CurrentSelectRole.Role_type == "D")     //医生站
            {
                DataInit.IniTreeViewByDoctor(trvInpatientManager.Nodes);
            }
            else
            {
                DataInit.IniTreeView(trvInpatientManager.Nodes);
            }

            /*
             *登录成功后，默认选中病人树的‘科室病人'节点
             */
            SelectedProvedNode(trvInpatientManager.Nodes, selectNode, trvInpatientManager);
            trvInpatientManager.ExpandAll();
        }
        /// <summary>
        /// 刷新后，选中刷新前的节点
        /// </summary>
        /// <param name="nodes">当前树节点集合</param>
        /// <param name="node">刷新前的节点</param>
        /// <param name="tvObject">当前操作的树对象</param>
        private void SelectedProvedNode(NodeCollection nodes, Node node, AdvTree tvObject)
        {
            if (node != null)
            {
                foreach (Node childNode in nodes)
                {
                    if (childNode.Name == node.Name)
                    {
                        if (node.Parent != null && node.Parent.Name == childNode.Parent.Name)
                        {
                            tvObject.SelectedNode = childNode;
                            tvObject.SelectedNode = childNode;
                            break;
                        }
                    }
                    if (childNode.Nodes.Count > 0)
                        SelectedProvedNode(childNode.Nodes, node, tvObject);
                }
            }
        }


        ///// <summary>
        ///// 获得术后病程记录下面所有病人文书的节点
        ///// </summary>
        ///// <param name="nodes">当前节点下的所有文书内容</param>
        ///// <returns>返回Patient_Doc集合</returns>
        //private  string[,] GetNodes(int text_Id)
        //{
        //    string sql = "select a.tid,a.patients_doc,a.textname from t_patients_doc a " +
        //                 " left join t_quality_text b on a.tid=b.tid " +
        //                 " where a.pid='" + currentPatient.PId + "'and a.textkind_id=" + text_Id + " order by a.tid";
        //    DataSet ds = App.GetDataSet(sql);
        //    string[,] arrs = null;
        //    if (ds != null)
        //    {
        //        DataTable dt = ds.Tables[0];
        //        if (dt != null)
        //        {
        //            arrs = new string[dt.Rows.Count, 2];
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                arrs[i, 0] = dt.Rows[i]["patients_doc"].ToString();
        //                arrs[i, 1] = dt.Rows[i]["tid"].ToString();
        //            }
        //        }
        //    }
        //    return arrs;
        //}
        /// <summary>
        /// 设置标题，住院病程记录的文书id=103,
        /// 下面所有文书标题为病程记录;
        /// 其他的文书的标题，则根据文书名称来显示
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <returns></returns>
        private string GetTextTitle(Node node)
        {
            string textTitle = "";
            try
            {
                if (node != null)
                {
                    textTitle = node.Text;
                }
                if (node.Parent.Parent != null)
                {
                    if (node.Parent.Name == "103" || node.Name == "103"  //住院病程记录文书id
                        || node.Parent.Parent.Name == "103")
                    {
                        if (node.Parent.Name == "134" || node.Name == "134" ||
                           node.Parent.Parent.Name == "134")  //术前小结
                        {
                            textTitle = "手术前小结";
                        }
                        else
                        {
                            textTitle = "病程记录";
                        }
                    }
                    else
                    {
                        if (node.Tag.GetType().Name.Contains("Patient_Doc"))
                        {
                            textTitle = node.Parent.Text;
                        }
                        else
                        {
                            textTitle = node.Text;
                        }
                        //textTitle = node.Text;
                    }
                    return textTitle;
                }
                else if (node.Parent != null)
                {
                    if (node.Parent.Name == "103" || node.Name == "103")
                    {
                        if (node.Parent.Name == "134" || node.Name == "134")//术前小结
                        {
                            textTitle = "手术前小结";
                        }
                        else
                        {
                            textTitle = "病程记录";
                        }
                    }
                    else
                    {
                        if (node.Tag.GetType().Name.Contains("Patient_Doc"))
                        {
                            textTitle = node.Parent.Text;
                        }
                        else
                        {
                            textTitle = node.Text;
                        }
                    }
                    return textTitle;
                }
            }
            catch (Exception)
            {
            }
            return textTitle;
        }
        //切换视图
        private void btnSwitch_Click_1(object sender, EventArgs e)
        {
            if (trvInpatientManager.SelectedNode != null)
            {
                //this.panel1.Controls.Clear();
                if (DataInit.ViewSwitch == 0)
                {
                    DataInit.ViewSwitch = 1;
                    tabitPatinetCard.Text = "病人列表";
                    //gbxInpatientInfo.Text = "病人列表";
                }
                else
                {
                    DataInit.ViewSwitch = 0;
                    tabitPatinetCard.Text = "病人小卡";
                    //gbxInpatientInfo.Text = "病人小卡";
                }
            }
            else
            {
                App.Msg("请选择一个科室！");
                return;
            }
            try
            {
                RefCard();
            }
            catch (Exception ex)
            {

                App.MsgErr("视图切换：" + ex.Message);
            }
        }
        //病人树右键菜单可见控制
        private void ctmnspReflash_Opening(object sender, CancelEventArgs e)
        {
            if (App.UserAccount.UserInfo.User_id == "1000000002")
            {
                添加病人toolStripMenuItem.Visible = true;
            }
            else
            {
                添加病人toolStripMenuItem.Visible = false;
            }
            //if (trvInpatientManager.SelectedNode == null)
            //{
            //    this.入区ToolStripMenuItem.Visible = false;
            //    this.历史病历查看ToolStripMenuItem.Visible = false;
            //    this.转入ToolStripMenuItem.Visible = false;
            //    this.退回转出科室ToolStripMenuItem.Visible = false;

            //    //this.修改入区时间ToolStripMenuItem.Visible = false;
            //    this.换床ToolStripMenuItem.Visible = false;
            //    this.更换管床医生ToolStripMenuItem.Visible = false;
            //    this.出区ToolStripMenuItem.Visible = false;
            //    this.挂床ToolStripMenuItem.Visible = false;
            //    this.转出ToolStripMenuItem.Visible = false;
            //    this.回收ToolStripMenuItem.Visible = false;
            //    this.取消转科ToolStripMenuItem.Visible = false;
            //    this.tlspmnitApply.Visible = false;
            //    this.病人信息ToolStripMenuItem.Visible = false;
            //    this.手术ToolStripMenuItem1.Visible = false;
            //    this.tlspmnitReflash.Visible = false;
            //}
            //else
            //{
            //    this.tlspmnitReflash.Visible = true;
            //    医嘱单ToolStripMenuItem.Visible = true;
            //    if (App.UserAccount.CurrentSelectRole.Role_type == "N" && App.UserAccount.CurrentSelectRole.Role_name.Contains("护士长"))
            //    {
            //        this.修改入区时间ToolStripMenuItem.Visible = true;
            //    }
            //    else
            //    {
            //        this.修改入区时间ToolStripMenuItem.Visible = false;
            //    }
            //}
        }

        /// <summary>
        /// 是否可以忽略空行
        /// </summary>
        /// <param name="node">当前选中的节点</param>
        /// <returns>true忽略，false不忽略</returns>
        private bool IsNeglectLine(Node node)
        {
            bool NeglectLin = true;
            if (node != null)
            {
                if (node.Tag.ToString().Contains("Class_Text"))//文书节点
                {
                    Class_Text class_Text = node.Tag as Class_Text;
                    if (class_Text.Txxttype == "915")//知情同意书
                    {
                        NeglectLin = false;
                    }
                }
                else if (node.Tag.ToString().Contains("Patient_Doc"))//文书内容节点
                {
                    if (node.Parent != null)
                    {
                        Class_Text class_Text = node.Parent.Tag as Class_Text;
                        if (class_Text.Txxttype == "915")//知情同意书
                        {
                            NeglectLin = false;
                        }
                    }
                }
            }
            return NeglectLin;
        }
        /// <summary>
        /// 获得病人
        /// </summary>
        /// <param name="bed_Id">床号id</param>
        /// <returns></returns>
        private InPatientInfo GetInpatientByTree(int bed_Id)
        {
            InPatientInfo old_inpatient = null;
            for (int i = 0; i < trvInpatientManager.Nodes[0].Nodes.Count; i++)
            {
                if (trvInpatientManager.Nodes[0].Nodes[i].Name == "tnSection_patient")
                {
                    Node tempnode = trvInpatientManager.Nodes[0].Nodes[i];
                    foreach (Node node in tempnode.Nodes)
                    {
                        for (int j = 0; j < node.Nodes.Count; j++)
                        {
                            InPatientInfo inpat = node.Nodes[j].Tag as InPatientInfo;
                            if (inpat.Sick_Bed_Id == bed_Id)
                            {
                                old_inpatient = inpat;
                                break;
                            }
                        }
                    }

                }
            }
            //foreach (TreeNode node in trvInpatientManager.Nodes[0].Nodes["tnSection_patient"].Nodes)
            //{
            //    for (int i = 0; i < node.Nodes.Count; i++)
            //    {
            //        InPatientInfo inpat = node.Nodes[i].Tag as InPatientInfo;
            //        if (inpat.Sick_Bed_Id == bed_Id)
            //        {
            //            old_inpatient = inpat;
            //            break;
            //        }
            //    }
            //}
            return old_inpatient;
        }

        private void 检验报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                InPatientInfo inPatient = null;
                node = (Node)trvInpatientManager.SelectedNode.DeepCopy();
                if (node != null)
                {
                    inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                }
                if (inPatient != null)
                {
                    FrmLis fc = new FrmLis(inPatient.PId);
                    App.FormStytleSet(fc, false);
                    fc.Show();
                }
            }
            catch
            {
                App.MsgErr("请先选择病人!");
            }
        }

        private void 病理报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //frmBljc            
            try
            {
                InPatientInfo inPatient = null;
                node = (Node)trvInpatientManager.SelectedNode.DeepCopy();
                if (node != null)
                {
                    inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                }
                if (inPatient != null)
                {
                    frmBljc fc = new frmBljc(inPatient.PId);
                    App.FormStytleSet(fc, false);
                    fc.Show();
                }
            }
            catch
            {
                App.MsgErr("请先选择病人!");
            }
        }

        private void 修改入区时间ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void 医嘱单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                InPatientInfo inPatient = null;
                node = (Node)trvInpatientManager.SelectedNode.DeepCopy();
                if (node != null)
                {
                    inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                }
                if (inPatient != null)
                {
                    frmYZ fc = new frmYZ(inPatient);
                    App.FormStytleSet(fc, false);
                    fc.Show();
                }
            }
            catch
            {
                App.MsgErr("请先选择病人或当前病人没有数据!");
            }
        }


        //DllTest，我们的动态链接库
        //[DllImport("XEFORHIS.dll")]
        //public static extern bool XePacsCall(int nPatintIDType, string lpszID, int nCallType);
        //[DllImport("XEFORHIS.dll")]
        //public static extern bool XePacsInit();
        //[DllImport("XEFORHIS.dll")]
        //public static extern bool XePacsRelease();

        /// <summary>
        /// 影像信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 影像StripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                InPatientInfo inPatient = null;
                node = (Node)trvInpatientManager.SelectedNode.DeepCopy();
                if (node != null)
                {
                    inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                }
                if (inPatient != null)
                {
                    Bifrost.HisInStance.frm_Pasc fc = new Bifrost.HisInStance.frm_Pasc(inPatient);
                    //frm_Pacs fc = new frmPacs(inPatient);
                    App.FormStytleSet(fc, false);
                    fc.Show();
                }
            }
            catch
            {
                //App.MsgErr("请先选择病人!");
            }
        }

        /// <summary>
        /// 手术麻醉报告查阅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 手麻报告查阅toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                InPatientInfo inPatient = null;
                node = (Node)trvInpatientManager.SelectedNode.DeepCopy();
                if (node != null)
                {
                    inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                }
                if (inPatient != null)
                {
                    //参数	    示例	    说明	            备注
                    //IP	    175.16.8.68	服务指向	
                    //Type	    Patient_id	病人ID号	        根据HIS
                    //Visit_id	Visit_id	住院次数或住院标识	根据HIS
                    //Mr_class	1001	    1001:麻醉
                    //His_no		        麻醉：his手术流水号 His手术流水号
                    //                      （可为空）	        （可为空）
                    //http://175.16.8.68/DocareInterfaceV4/main/Patient_history.aspx?patient_id=ZY010013134663&visit_id=1&mr_class=1001&his_no=27103

                    string urlSSMZ = @"http://192.168.2.217/WebSite1/SSMZ/Patient_history.aspx?patient_id=" + inPatient.His_id + "&visit_id=" + inPatient.InHospital_count + "&mr_class=1001&his_no=";
                    //Bifrost.HisInStance.frmPicShow fc = new Bifrost.HisInStance.frmPicShow(urlSSMZ);
                    //App.FormStytleSet(fc, false);
                    //fc.Show();

                    System.Diagnostics.Process.Start(urlSSMZ);
                }
            }
            catch
            {
                App.MsgErr("请先选择病人!");
            }
        }

        /// <summary>
        /// 判断当前用户是否可以删除文书
        /// </summary>
        /// <param name="account_Id">当前用户的id</param>
        /// <param name="tid">文书id</param>
        /// <returns>true可以删除，false不能删除</returns>
        private bool IsDelete(string account_Id, string tid)
        {
            bool isFlag = false;
            //获得当前文书的创建人
            string sql = "select createid from t_patients_doc where tid=" + tid + "";
            string add_Id = App.ReadSqlVal(sql, 0, "createid");
            if (account_Id == add_Id || add_Id == "")
            {
                isFlag = true;
            }
            return isFlag;
        }

        private void 添加病人toolStripMenuItem_Click(object sender, EventArgs e)
        {
            //20130911:已自动同步,停止手动添加病人
            //frmPatientIN patientIn = new frmPatientIN();
            //App.UsControlStyle(patientIn);
            //patientIn.ShowDialog();

            //bool YN=false;
            //if (App.UserAccount.CurrentSelectRole.Role_type=="D")
            //{//医生
            //    YN = App.UserAccount.CurrentSelectRole.Section_name.Contains("北院神经");
            //}
            //else if (App.UserAccount.CurrentSelectRole.Role_type == "N")
            //{//护士
            //    YN =App.UserAccount.CurrentSelectRole.Sickarea_name.Contains("北院十四病室") || App.UserAccount.CurrentSelectRole.Sickarea_name.Contains("北院一病室");
            //}
            //if (YN)
            //{//目前北院神经内外科测试使用
            //    frmPatientIN patientIn = new frmPatientIN();
            //    App.UsControlStyle(patientIn);
            //    patientIn.ShowDialog();
            //}
            //else
            //{
            frmPatientIN_Old patientIn = new frmPatientIN_Old();
            App.UsControlStyle(patientIn);
            patientIn.ShowDialog();
            //}
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void advTree1_AfterNodeSelect(object sender, AdvTreeNodeEventArgs e)
        {
            if (trvInpatientManager.SelectedNode != null)
            {
                CurrentSelectNode = trvInpatientManager.SelectedNode;
                currentPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                if (trvInpatientManager.SelectedNode.Parent != null)
                {
                    #region 解决病人树切换速度慢的问题
                    InPatientInfo patient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;

                    if (patient == null)//当选中的节点的Tag为null时，说明选中节点不是病人 
                    {
                        RefCard();
                    }
                    if (patient != null)//当选中的是病人时，如果与上一次选择的病人是不同的父节点，则刷新小卡
                    {
                        if (pranteText != trvInpatientManager.SelectedNode.Parent.Text)
                        {

                            RefCard();
                        }
                    }
                    pranteText = trvInpatientManager.SelectedNode.Parent.Text;
                    #endregion
                    //if (App.UserAccount.CurrentSelectRole.Sickarea_Id != "" &&
                    //    App.UserAccount.CurrentSelectRole.Sickarea_Id != "0")
                    //{
                    if (App.UserAccount.CurrentSelectRole.Role_type != "D")
                    {
                        if (trvInpatientManager.SelectedNode.Parent.Text.Equals("待入区病人"))
                        {
                            this.入区ToolStripMenuItem.Visible = true;
                            this.历史病历查看ToolStripMenuItem.Visible = true;
                            检验报告ToolStripMenuItem.Visible = true;
                            this.刷新tlspmnitReflash.Visible = true;
                            病理报告ToolStripMenuItem.Visible = true;
                            医嘱单toolStripMenuItem.Visible = true;
                            影像StripMenuItem.Visible = true;
                            手麻报告查阅toolStripMenuItem1.Visible = false;
                            病人检查趋势图toolStripMenuItem1.Visible = true;
                            病人综合视图toolStripMenuItem1.Visible = true;
                            this.历史病案查阅toolStripMenuItem1.Visible = false;
                            this.转入ToolStripMenuItem.Visible = false;
                            this.退回转出科室ToolStripMenuItem.Visible = false;
                            this.修改入区时间ToolStripMenuItem.Visible = false;
                            this.换床ToolStripMenuItem.Visible = false;
                            this.更换管床医生ToolStripMenuItem.Visible = false;
                            this.出区ToolStripMenuItem.Visible = false;
                            this.挂床ToolStripMenuItem.Visible = false;
                            this.转出ToolStripMenuItem.Visible = false;
                            this.回收ToolStripMenuItem.Visible = false;
                            this.取消转科ToolStripMenuItem.Visible = false;
                            this.会诊申请tlspmnitApply.Visible = false;
                            this.病人信息ToolStripMenuItem.Visible = false;
                            //this.手术ToolStripMenuItem1.Visible = false;
                            this.添加病人toolStripMenuItem.Visible = false;
                            this.重新转出toolStripMenuItem1.Visible = false;
                            this.手动转区toolStripMenuItem1.Visible = false;
                        }
                        else if (trvInpatientManager.SelectedNode.Parent.Text.Equals("待转入病人"))
                        {
                            this.转入ToolStripMenuItem.Visible = true;
                            this.刷新tlspmnitReflash.Visible = true;
                            this.退回转出科室ToolStripMenuItem.Visible = true;
                            检验报告ToolStripMenuItem.Visible = true;
                            病理报告ToolStripMenuItem.Visible = true;
                            医嘱单toolStripMenuItem.Visible = true;
                            影像StripMenuItem.Visible = true;
                            手麻报告查阅toolStripMenuItem1.Visible = false;
                            病人检查趋势图toolStripMenuItem1.Visible = true;
                            病人综合视图toolStripMenuItem1.Visible = true;
                            this.历史病案查阅toolStripMenuItem1.Visible = false;

                            this.入区ToolStripMenuItem.Visible = false;
                            this.修改入区时间ToolStripMenuItem.Visible = false;
                            this.换床ToolStripMenuItem.Visible = false;
                            this.更换管床医生ToolStripMenuItem.Visible = false;
                            this.出区ToolStripMenuItem.Visible = false;
                            this.挂床ToolStripMenuItem.Visible = false;
                            this.转出ToolStripMenuItem.Visible = false;
                            this.回收ToolStripMenuItem.Visible = false;
                            this.取消转科ToolStripMenuItem.Visible = false;
                            this.会诊申请tlspmnitApply.Visible = false;
                            this.病人信息ToolStripMenuItem.Visible = false;
                            //this.手术ToolStripMenuItem1.Visible = false;
                            this.历史病历查看ToolStripMenuItem.Visible = false;
                            this.添加病人toolStripMenuItem.Visible = false;
                            this.重新转出toolStripMenuItem1.Visible = false;
                            this.手动转区toolStripMenuItem1.Visible = false;

                        }
                        else if (trvInpatientManager.SelectedNode.Parent.Text.Equals("我的病人"))
                        {
                            //this.修改入区时间ToolStripMenuItem.Visible = true;
                            this.换床ToolStripMenuItem.Visible = true;
                            this.刷新tlspmnitReflash.Visible = true;
                            this.更换管床医生ToolStripMenuItem.Visible = true;
                            this.出区ToolStripMenuItem.Visible = false;
                            this.挂床ToolStripMenuItem.Visible = true;
                            this.转出ToolStripMenuItem.Visible = true;
                            检验报告ToolStripMenuItem.Visible = true;
                            病理报告ToolStripMenuItem.Visible = true;
                            医嘱单toolStripMenuItem.Visible = true;
                            影像StripMenuItem.Visible = true;
                            手麻报告查阅toolStripMenuItem1.Visible = false;
                            病人检查趋势图toolStripMenuItem1.Visible = true;
                            病人综合视图toolStripMenuItem1.Visible = true;
                            this.历史病案查阅toolStripMenuItem1.Visible = true;

                            this.转入ToolStripMenuItem.Visible = false;
                            this.退回转出科室ToolStripMenuItem.Visible = false;
                            this.回收ToolStripMenuItem.Visible = false;
                            this.取消转科ToolStripMenuItem.Visible = false;
                            this.入区ToolStripMenuItem.Visible = false;
                            this.会诊申请tlspmnitApply.Visible = false;
                            this.病人信息ToolStripMenuItem.Visible = false;
                            //this.手术ToolStripMenuItem1.Visible = false;
                            this.历史病历查看ToolStripMenuItem.Visible = false;
                            this.添加病人toolStripMenuItem.Visible = false;
                            this.重新转出toolStripMenuItem1.Visible = false;
                            this.手动转区toolStripMenuItem1.Visible = false;
                        }
                        else if (DataInit.IsSectionName(trvInpatientManager.SelectedNode.Parent.Text))
                        {
                            //this.修改入区时间ToolStripMenuItem.Visible = true;
                            this.转出ToolStripMenuItem.Visible = true;
                            this.刷新tlspmnitReflash.Visible = true;
                            this.换床ToolStripMenuItem.Visible = true;
                            this.更换管床医生ToolStripMenuItem.Visible = true;
                            this.出区ToolStripMenuItem.Visible = true;
                            this.挂床ToolStripMenuItem.Visible = false;
                            this.病人信息ToolStripMenuItem.Visible = true;
                            //this.手术ToolStripMenuItem1.Visible = true;
                            检验报告ToolStripMenuItem.Visible = true;
                            病理报告ToolStripMenuItem.Visible = true;
                            医嘱单toolStripMenuItem.Visible = true;
                            影像StripMenuItem.Visible = true;
                            手麻报告查阅toolStripMenuItem1.Visible = false;
                            病人检查趋势图toolStripMenuItem1.Visible = true;
                            病人综合视图toolStripMenuItem1.Visible = true;
                            this.历史病案查阅toolStripMenuItem1.Visible = true;
                            this.修改入区时间ToolStripMenuItem.Visible = false;

                            this.转入ToolStripMenuItem.Visible = false;
                            this.退回转出科室ToolStripMenuItem.Visible = false;
                            this.回收ToolStripMenuItem.Visible = false;
                            this.取消转科ToolStripMenuItem.Visible = false;
                            this.入区ToolStripMenuItem.Visible = false;
                            this.会诊申请tlspmnitApply.Visible = false;
                            //this.手术ToolStripMenuItem1.Visible = false;
                            this.历史病历查看ToolStripMenuItem.Visible = false;
                            this.添加病人toolStripMenuItem.Visible = false;
                            this.重新转出toolStripMenuItem1.Visible = false;
                            this.手动转区toolStripMenuItem1.Visible = false;
                        }
                        else if ((trvInpatientManager.SelectedNode.Parent.Text.Equals("已出院病人")
                       && trvInpatientManager.SelectedNode.Name != "tnMyOutPatient")
                            ||trvInpatientManager.SelectedNode.Parent.Name=="tnMyOutPatient")
                        {
                            this.回收ToolStripMenuItem.Visible = true;
                            检验报告ToolStripMenuItem.Visible = true;
                            this.刷新tlspmnitReflash.Visible = true;
                            病理报告ToolStripMenuItem.Visible = true;
                            医嘱单toolStripMenuItem.Visible = true;
                            影像StripMenuItem.Visible = true;
                            手麻报告查阅toolStripMenuItem1.Visible = false;
                            病人检查趋势图toolStripMenuItem1.Visible = true;
                            病人综合视图toolStripMenuItem1.Visible = true;
                            this.历史病案查阅toolStripMenuItem1.Visible = false;


                            this.修改入区时间ToolStripMenuItem.Visible = false;
                            this.转出ToolStripMenuItem.Visible = false;
                            this.换床ToolStripMenuItem.Visible = false;
                            this.更换管床医生ToolStripMenuItem.Visible = false;
                            this.出区ToolStripMenuItem.Visible = false;
                            this.挂床ToolStripMenuItem.Visible = false;
                            this.转入ToolStripMenuItem.Visible = false;
                            this.退回转出科室ToolStripMenuItem.Visible = false;
                            this.取消转科ToolStripMenuItem.Visible = false;
                            this.入区ToolStripMenuItem.Visible = false;
                            this.会诊申请tlspmnitApply.Visible = false;
                            this.病人信息ToolStripMenuItem.Visible = true;
                            //this.手术ToolStripMenuItem1.Visible = false;
                            this.历史病历查看ToolStripMenuItem.Visible = false;
                            this.添加病人toolStripMenuItem.Visible = false;
                            this.重新转出toolStripMenuItem1.Visible = false;
                            this.手动转区toolStripMenuItem1.Visible = false;
                        }
                        else if (trvInpatientManager.SelectedNode.Parent.Text.Equals("已转出病人"))
                        {
                            this.取消转科ToolStripMenuItem.Visible = true;
                            this.刷新tlspmnitReflash.Visible = true;
                            检验报告ToolStripMenuItem.Visible = true;
                            病理报告ToolStripMenuItem.Visible = true;
                            医嘱单toolStripMenuItem.Visible = true;
                            影像StripMenuItem.Visible = true;
                            手麻报告查阅toolStripMenuItem1.Visible = false;
                            病人检查趋势图toolStripMenuItem1.Visible = true;
                            病人综合视图toolStripMenuItem1.Visible = true;
                            this.历史病案查阅toolStripMenuItem1.Visible = false;

                            this.回收ToolStripMenuItem.Visible = false;
                            this.修改入区时间ToolStripMenuItem.Visible = false;
                            this.转出ToolStripMenuItem.Visible = false;
                            this.换床ToolStripMenuItem.Visible = false;
                            this.更换管床医生ToolStripMenuItem.Visible = false;
                            this.出区ToolStripMenuItem.Visible = false;
                            this.挂床ToolStripMenuItem.Visible = false;
                            this.转入ToolStripMenuItem.Visible = false;
                            this.退回转出科室ToolStripMenuItem.Visible = false;
                            this.入区ToolStripMenuItem.Visible = false;
                            this.会诊申请tlspmnitApply.Visible = false;
                            this.病人信息ToolStripMenuItem.Visible = false;
                            //this.手术ToolStripMenuItem1.Visible = false;
                            this.历史病历查看ToolStripMenuItem.Visible = false;
                            this.添加病人toolStripMenuItem.Visible = false;
                            this.手动转区toolStripMenuItem1.Visible = false;
                        }
                        else
                        {
                            病人检查趋势图toolStripMenuItem1.Visible = true;
                            this.刷新tlspmnitReflash.Visible = true;
                            病人综合视图toolStripMenuItem1.Visible = true;
                            this.历史病案查阅toolStripMenuItem1.Visible = false;
                            换床ToolStripMenuItem.Visible = true;
                            检验报告ToolStripMenuItem.Visible = false;
                            病理报告ToolStripMenuItem.Visible = false;
                            医嘱单toolStripMenuItem.Visible = false;
                            影像StripMenuItem.Visible = false;//lianwei
                            手麻报告查阅toolStripMenuItem1.Visible = false;
                            this.取消转科ToolStripMenuItem.Visible = false;
                            this.回收ToolStripMenuItem.Visible = false;
                            this.修改入区时间ToolStripMenuItem.Visible = false;
                            this.转出ToolStripMenuItem.Visible = false;
                            this.换床ToolStripMenuItem.Visible = false;
                            this.更换管床医生ToolStripMenuItem.Visible = false;
                            this.出区ToolStripMenuItem.Visible = false;
                            this.挂床ToolStripMenuItem.Visible = false;
                            this.转入ToolStripMenuItem.Visible = false;
                            this.退回转出科室ToolStripMenuItem.Visible = false;
                            this.入区ToolStripMenuItem.Visible = false;
                            this.会诊申请tlspmnitApply.Visible = false;
                            this.病人信息ToolStripMenuItem.Visible = false;
                            //this.手术ToolStripMenuItem1.Visible = false;
                            this.历史病历查看ToolStripMenuItem.Visible = false;
                            //this.添加病人toolStripMenuItem.Visible = true;
                            this.添加病人toolStripMenuItem.Visible = false;
                            this.手动转区toolStripMenuItem1.Visible = false;
                        }
                    }
                    else
                    {
                        if (((trvInpatientManager.SelectedNode.Parent.Text.Equals("已出院病人")
                       && trvInpatientManager.SelectedNode.Name != "tnMyOutPatient")
                            || trvInpatientManager.SelectedNode.Parent.Name == "tnMyOutPatient") ||
                            trvInpatientManager.SelectedNode.Parent.Text.Equals("我的病人") ||
                            App.UserAccount.CurrentSelectRole.Section_name.Contains(trvInpatientManager.SelectedNode.Parent.Text))
                        {

                            this.取消转科ToolStripMenuItem.Visible = false;
                            this.历史病历查看ToolStripMenuItem.Visible = false;
                            this.回收ToolStripMenuItem.Visible = false;
                            this.修改入区时间ToolStripMenuItem.Visible = false;
                            this.转出ToolStripMenuItem.Visible = false;
                            if (App.CurrentHospitalId == 201)
                            {
                                this.换床ToolStripMenuItem.Visible = true;

                            }
                            else
                            {
                                this.换床ToolStripMenuItem.Visible = false;

                            }
                            this.更换管床医生ToolStripMenuItem.Visible = false;
                            this.出区ToolStripMenuItem.Visible = false;
                            this.挂床ToolStripMenuItem.Visible = false;
                            this.转入ToolStripMenuItem.Visible = false;
                            this.退回转出科室ToolStripMenuItem.Visible = false;
                            this.入区ToolStripMenuItem.Visible = false;
                            //this.添加病人toolStripMenuItem.Visible = true;
                            this.添加病人toolStripMenuItem.Visible = false;
                            this.重新转出toolStripMenuItem1.Visible = false;
                            this.历史病案查阅toolStripMenuItem1.Visible = false;

                            检验报告ToolStripMenuItem.Visible = true;
                            病理报告ToolStripMenuItem.Visible = true;
                            医嘱单toolStripMenuItem.Visible = true;
                            this.刷新tlspmnitReflash.Visible = true;
                            影像StripMenuItem.Visible = true;
                            手麻报告查阅toolStripMenuItem1.Visible = false;
                            this.会诊申请tlspmnitApply.Visible = true;
                            this.病人信息ToolStripMenuItem.Visible = true;
                            //this.手术ToolStripMenuItem1.Visible = true;
                            病人检查趋势图toolStripMenuItem1.Visible = true;
                            病人综合视图toolStripMenuItem1.Visible = true;
                            文书授权toolStripMenuItem.Visible = true;
                            取消授权toolStripMenuItem.Visible = true;
                            if (patient.IsHaveRight)//已授权的文书启用取消授权
                            {
                                取消授权toolStripMenuItem.Enabled = true;
                            }
                            else
                            {
                                取消授权toolStripMenuItem.Enabled = false;
                            }
                            if (trvInpatientManager.SelectedNode.Parent.Text.Contains("北院消化内科")
                             || trvInpatientManager.SelectedNode.Parent.Text.Contains("北院肿瘤内科")
                             || trvInpatientManager.SelectedNode.Parent.Text.Contains("北院心血管介入科")
                             || trvInpatientManager.SelectedNode.Parent.Text.Contains("北院心血管CCU科"))
                            {
                                this.手动转区toolStripMenuItem1.Visible = true;
                            }
                            else
                            {
                                this.手动转区toolStripMenuItem1.Visible = false;
                            }
                        }
                        else
                        {
                            this.刷新tlspmnitReflash.Visible = true;
                            检验报告ToolStripMenuItem.Visible = false;
                            病理报告ToolStripMenuItem.Visible = false;
                            医嘱单toolStripMenuItem.Visible = false;
                            影像StripMenuItem.Visible = false;//lianwei
                            手麻报告查阅toolStripMenuItem1.Visible = false;
                            this.历史病历查看ToolStripMenuItem.Visible = false;
                            this.取消转科ToolStripMenuItem.Visible = false;
                            this.回收ToolStripMenuItem.Visible = false;
                            this.修改入区时间ToolStripMenuItem.Visible = false;
                            this.转出ToolStripMenuItem.Visible = false;
                            this.换床ToolStripMenuItem.Visible = false;
                            this.更换管床医生ToolStripMenuItem.Visible = false;
                            this.出区ToolStripMenuItem.Visible = false;
                            this.挂床ToolStripMenuItem.Visible = false;
                            this.转入ToolStripMenuItem.Visible = false;
                            this.退回转出科室ToolStripMenuItem.Visible = false;
                            this.入区ToolStripMenuItem.Visible = false;
                            this.会诊申请tlspmnitApply.Visible = false;
                            this.病人信息ToolStripMenuItem.Visible = false;
                            //this.手术ToolStripMenuItem1.Visible = false;
                            this.重新转出toolStripMenuItem1.Visible = false;
                            病人检查趋势图toolStripMenuItem1.Visible = true;
                            病人综合视图toolStripMenuItem1.Visible = true;
                            文书授权toolStripMenuItem.Visible = false;
                            取消授权toolStripMenuItem.Visible = false;
                            this.手动转区toolStripMenuItem1.Visible = false;
                            this.历史病案查阅toolStripMenuItem1.Visible = false;

                            if (trvInpatientManager.SelectedNode.Text == "我的病人")
                            {
                                //this.添加病人toolStripMenuItem.Visible = true;
                                this.添加病人toolStripMenuItem.Visible = false;
                            }
                            else
                            {
                                this.添加病人toolStripMenuItem.Visible = false;
                            }
                        }
                        if (trvInpatientManager.SelectedNode.Parent.Text.Equals("待转入病人"))
                        {
                            this.退回转出科室ToolStripMenuItem.Visible = true;
                            this.转入ToolStripMenuItem.Visible = true;
                            病人检查趋势图toolStripMenuItem1.Visible = true;
                            病人综合视图toolStripMenuItem1.Visible = true;
                            this.刷新tlspmnitReflash.Visible = true;
                            this.历史病案查阅toolStripMenuItem1.Visible = false;

                            检验报告ToolStripMenuItem.Visible = true;
                            医嘱单toolStripMenuItem.Visible = true;
                            影像StripMenuItem.Visible = true;


                        }
                        else if (trvInpatientManager.SelectedNode.Parent.Text.Equals("我的病人") ||

                            App.UserAccount.CurrentSelectRole.Section_name.Contains(trvInpatientManager.SelectedNode.Parent.Text))
                        {
                            this.转出ToolStripMenuItem.Visible = true;
                            //this.转入ToolStripMenuItem.Visible = true;
                            this.出区ToolStripMenuItem.Visible = true;
                            if (App.CurrentHospitalId == 201)
                            {
                                this.换床ToolStripMenuItem.Visible = true;

                            }
                            else
                            {
                                this.换床ToolStripMenuItem.Visible = false;

                            }
                            this.更换管床医生ToolStripMenuItem.Visible = true;
                            病人检查趋势图toolStripMenuItem1.Visible = true;
                            病人综合视图toolStripMenuItem1.Visible = true;
                            this.刷新tlspmnitReflash.Visible = true;
                            this.历史病案查阅toolStripMenuItem1.Visible = true;

                        }
                        else if (trvInpatientManager.SelectedNode.Parent.Text.Equals("已出院病人"))
                        {
                            this.更换管床医生ToolStripMenuItem.Visible = false;
                            this.回收ToolStripMenuItem.Visible = true;
                            this.刷新tlspmnitReflash.Visible = true;
                            病人检查趋势图toolStripMenuItem1.Visible = true;
                            病人综合视图toolStripMenuItem1.Visible = true;
                            this.历史病案查阅toolStripMenuItem1.Visible = false;

                        }
                        else if (trvInpatientManager.SelectedNode.Parent.Text.Equals("已转出病人"))
                        {
                            检验报告ToolStripMenuItem.Visible = true;
                            医嘱单toolStripMenuItem.Visible = true;
                            影像StripMenuItem.Visible = true;

                            this.取消转科ToolStripMenuItem.Visible = true;
                            this.历史病案查阅toolStripMenuItem1.Visible = false;
                            this.刷新tlspmnitReflash.Visible = true;
                            if (trvInpatientManager.SelectedNode.Text.Contains("退回"))
                            {
                                this.重新转出toolStripMenuItem1.Visible = true;
                                病人检查趋势图toolStripMenuItem1.Visible = true;
                                病人综合视图toolStripMenuItem1.Visible = true;
                            }
                            else
                            {
                                this.重新转出toolStripMenuItem1.Visible = false;
                                病人检查趋势图toolStripMenuItem1.Visible = true;
                                病人综合视图toolStripMenuItem1.Visible = true;
                            }
                        }


                    }
                    //}
                }
            }
            else
            {
                检验报告ToolStripMenuItem.Visible = false;
                病理报告ToolStripMenuItem.Visible = false;
                医嘱单toolStripMenuItem.Visible = false;
                影像StripMenuItem.Visible = true;
                手麻报告查阅toolStripMenuItem1.Visible = false;
                this.历史病案查阅toolStripMenuItem1.Visible = false;

                this.历史病历查看ToolStripMenuItem.Visible = false;
                this.取消转科ToolStripMenuItem.Visible = false;
                this.回收ToolStripMenuItem.Visible = false;
                this.修改入区时间ToolStripMenuItem.Visible = false;
                this.转出ToolStripMenuItem.Visible = false;
                this.换床ToolStripMenuItem.Visible = false;
                this.更换管床医生ToolStripMenuItem.Visible = false;
                this.出区ToolStripMenuItem.Visible = false;
                this.挂床ToolStripMenuItem.Visible = false;
                this.转入ToolStripMenuItem.Visible = false;
                this.退回转出科室ToolStripMenuItem.Visible = false;
                this.入区ToolStripMenuItem.Visible = false;
                this.会诊申请tlspmnitApply.Visible = false;
                this.病人信息ToolStripMenuItem.Visible = false;
                ////this.手术ToolStripMenuItem1.Visible = false;

                this.病人检查趋势图toolStripMenuItem1.Visible = false;
                this.病人综合视图toolStripMenuItem1.Visible = false;
                this.文书授权toolStripMenuItem.Visible = false;
                this.取消授权toolStripMenuItem.Visible = false;
                //App.Msg("请选择病人");
                return;
            }
            if (trvInpatientManager.SelectedNode.Tag != null)
            {
                //if (trvInpatientManager.SelectedNode.Text == "已出院病人" || (trvInpatientManager.SelectedNode.Parent != null && trvInpatientManager.SelectedNode.Parent.Text == "已出院病人"))
                //{
                InPatientInfo inpat = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                string sex = null;
                if (inpat.Gender_Code.Equals("0") || inpat.Gender_Code.Equals("男"))
                {
                    sex = "男";
                }
                else
                {
                    sex = "女";
                }
                App.SetMainFrmMsgToolBarText(inpat.Sick_Bed_Name + "床  " + "ID:" + inpat.Id.ToString() +
                                            "  住院号:" + inpat.PId + "  姓名:" + inpat.Patient_Name +
                                            "  性别:" + sex + "  年龄:" + inpat.Age.ToString() +
                                            "  入院时间:" + inpat.In_Time.ToString() +
                                            "  当前科室:" + inpat.Sick_Area_Name);
                //}
            }
            else
            {
                //App.SetMainFrmMsgToolBarText("");
                检验报告ToolStripMenuItem.Visible = false;
                病理报告ToolStripMenuItem.Visible = false;
                医嘱单toolStripMenuItem.Visible = false;
                影像StripMenuItem.Visible = false;//lianwei
                影像查看StripMenuItem.Visible = false;
                
                手麻报告查阅toolStripMenuItem1.Visible = false;
                病人检查趋势图toolStripMenuItem1.Visible = false;
                病人综合视图toolStripMenuItem1.Visible = false;
                this.历史病案查阅toolStripMenuItem1.Visible = false;
                this.历史病历查看ToolStripMenuItem.Visible = false;
                this.取消转科ToolStripMenuItem.Visible = false;
                this.回收ToolStripMenuItem.Visible = false;
                this.修改入区时间ToolStripMenuItem.Visible = false;
                this.转出ToolStripMenuItem.Visible = false;
                this.换床ToolStripMenuItem.Visible = false;
                this.更换管床医生ToolStripMenuItem.Visible = false;
                this.出区ToolStripMenuItem.Visible = false;
                this.挂床ToolStripMenuItem.Visible = false;
                this.转入ToolStripMenuItem.Visible = false;
                this.退回转出科室ToolStripMenuItem.Visible = false;
                this.入区ToolStripMenuItem.Visible = false;
                this.会诊申请tlspmnitApply.Visible = false;
                this.病人信息ToolStripMenuItem.Visible = false;
                //this.手术ToolStripMenuItem1.Visible = false;
                this.重新转出toolStripMenuItem1.Visible = false;
                this.文书授权toolStripMenuItem.Visible = false;
                取消授权toolStripMenuItem.Visible = false;
                this.刷新tlspmnitReflash.Visible = true;
                //this.刷新tlspmnitReflash

            }
            InPatientInfo _inpat = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
            if (_inpat == null) { return; }
            if (_inpat.IsHaveRight)//已授权的文书授权按钮变为查看授权，取消授权启用
            {
                this.文书授权toolStripMenuItem.Text = "查看授权";
                this.取消授权toolStripMenuItem.Enabled = true;
            }
            else
            {
                this.文书授权toolStripMenuItem.Text = "文书授权";
                this.取消授权toolStripMenuItem.Enabled = false;
            }
            #region 屏蔽按钮-镇平中医院
            病理报告ToolStripMenuItem.Visible = false;
            //影像StripMenuItem.Visible = false;//liawnei
            影像查看StripMenuItem.Visible = false;
            手麻报告查阅toolStripMenuItem1.Visible = false;
            病人检查趋势图toolStripMenuItem1.Visible = false;
            会诊申请tlspmnitApply.Visible = false;
            换床ToolStripMenuItem.Visible = false;
            #endregion
            toolTip1.SetToolTip(trvInpatientManager, "");
            //出院列表的临时提示框
            if (trvInpatientManager.SelectedNode != null)
            {
                if (trvInpatientManager.SelectedNode.Text == "已出院病人" || (trvInpatientManager.SelectedNode.Parent != null && trvInpatientManager.SelectedNode.Parent.Text == "已出院病人"))
                {
                    //toolTip1.SetToolTip(trvInpatientManager, "患者出院后第二日早8点病历自动归档收回，不可修改，若需修改请与病案室联系！");
                    toolTip1.Hide(trvInpatientManager);
                }
                else
                {
                    toolTip1.Hide(trvInpatientManager);
                }
            }
            else
            {
                toolTip1.Hide(trvInpatientManager);
            }
        }

        private void btnNotSign_Click(object sender, EventArgs e)
        {
            frmNotSignature frmNotSign = new frmNotSignature();
            frmNotSign.Show();
        }

        private void 病人综合视图toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            InPatientInfo currentPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
            frmPatientComView patientComView = new frmPatientComView(currentPatient);
            patientComView.ShowDialog();
        }

        private void 病人检查趋势图toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            InPatientInfo inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
            Base_Function.BLL_DOCTOR.Patient_Action_Manager.frmPatientProgress patientProgress = new Base_Function.BLL_DOCTOR.Patient_Action_Manager.frmPatientProgress(inPatient);
            patientProgress.ShowDialog();
        }


        //事件调用的方法
        void ucapply_browse_Book(object sender, Child_EventArgs e)
        {
            //DataInit.boolAgree = false;
            //this.currentPatient = DataInit.GetInpatientInfoByPid(e.Id.ToString());
            //this.Flag = "";
            //int id = e.Id;
            //if (e.State == "同意")
            //{
            //    if (App.UserAccount.UserInfo.User_id == e.User_Id)
            //    {
            //        DataInit.boolAgree = true;
            //        if (this.tctlNormolOperate.Tabs[0].Text == "常用操作")
            //        {
            //            this.tctlNormolOperate.SelectedTabIndex = 1;
            //        }
            //        else
            //        {
            //            this.tctlNormolOperate.SelectedTabIndex = 0;
            //        }
            //    }
            //    else
            //    {
            //        App.Msg("只有本人才能够浏览!");
            //    }

            //}
            //else
            //{
            //    App.Msg("请您选择审核同意的申请!");
            //}

        }

        //事件调用的方法
        void ucapply_browse_Book2(object sender, Child_EventArgs e)
        {
            
            if (App.UserAccount.UserInfo.User_id == e.User_Id)
            {
                DataInit.boolAgree = true;
                this.currentPatient = DataInit.GetInpatientInfoByPid(e.Id.ToString());
                currentPatient.PatientState = "借阅";//病人状态
                string pageText = "借阅" + " " + currentPatient.Patient_Name;
                //验证TabControl是否有重复
                for (int i = 0; i < tabControl_Patient.Tabs.Count; i++)
                {
                    if (pageText == tabControl_Patient.Tabs[i].Text)
                    {
                        tabControl_Patient.SelectedTabIndex = i;
                        return;
                    }
                }

                TabControlPanel tabctpnDoc = new TabControlPanel();
                tabctpnDoc.AutoScroll = true;
                TabItem pageDoc = new TabItem();
                pageDoc.Name = currentPatient.Id.ToString();
                pageDoc.Text = pageText;
                pageDoc.Click += new EventHandler(page_Click);
                pageDoc.Tag = currentPatient;

                if (!DataInit.IsAccessRights(currentPatient.Id))
                {
                    App.Msg("您的权限不足，请联系职能科室!");
                    return;
                }

                ucDoctorOperater fm = new ucDoctorOperater(currentPatient);
                fm.Dock = DockStyle.Fill;
                tabctpnDoc.Controls.Add(fm);
                tabctpnDoc.Dock = DockStyle.Fill;
                pageDoc.AttachedControl = tabctpnDoc;
                tabControl_Patient.Controls.Add(tabctpnDoc);
                tabControl_Patient.Tabs.Add(pageDoc);
                tabControl_Patient.Refresh();
                tabControl_Patient.SelectedTab = pageDoc;
            }
            else
            {
                App.Msg("提示: 只有申请人才能浏览!");
            }
        }

        /// <summary>
        /// 文书授权
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 文书授权toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            InPatientInfo patient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;

            if (patient != null && patient.Sick_Doctor_Id == App.UserAccount.UserInfo.User_id)
            {
                Bifrost.SYSTEMSET.frmTextRightSet frmTextRight = new Bifrost.SYSTEMSET.frmTextRightSet(patient.Id.ToString());
                frmTextRight.StartPosition = FormStartPosition.CenterParent;
                frmTextRight.ShowDialog();
                //授权界面关闭后，重新设置小卡背景色
                try
                {
                    string sql_docRight = "select a.patient_id from t_set_text_rights a inner join t_in_patient b on a.patient_id=b.id where b.section_id = " + App.UserAccount.CurrentSelectRole.Section_Id + " and a.end_time>sysdate and b.document_state is null";
                    DataSet ds = App.GetDataSet(sql_docRight);
                    if (ds != null)
                    {
                        DataTable dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            //循环文书授权表
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                //循环当前Panel中的小卡
                                for (int j = 0; j < ucHospitalIofn1.flowLayoutPanel1.Controls.Count; j++)
                                {
                                    //取出小卡的病人对象
                                    InPatientInfo patient_docRight = ucHospitalIofn1.flowLayoutPanel1.Controls[j].Tag as InPatientInfo;
                                    if (patient_docRight != null)
                                    {
                                        if (patient_docRight.Id == Convert.ToInt32(dt.Rows[i]["patient_id"]))
                                        {
                                            if (patient_docRight.IsChangeSection != 'T')
                                            {
                                                //ucHospitalIofn1.flowLayoutPanel1.Controls[j].BackgroundImage = global::Base_Function.Resource.card_Purple;
                                            }
                                            patient_docRight.IsHaveRight = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                App.Msg("只有管床医生可以授权！");
            }
        }

        /// <summary>
        /// 授权文书事件调用的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uCDocRight_browse_Book(object sender, DocRight_EventArgs e)
        {
            DataInit.isRightDoc = true;//授权文书
            rightDoc_functions = e.Functions;//获取权限
            //this.currentPatient = DataInit.GetInpatientInfoByPid(e.Id.ToString());
            //if (this.tctlNormolOperate.Tabs[0].Text == "常用操作")
            //{
            //    this.tctlNormolOperate.SelectedTabIndex = 1;
            //}
            //else
            //{
            //    this.tctlNormolOperate.SelectedTabIndex = 0;
            //}

            this.currentPatient = DataInit.GetInpatientInfoByPid(e.Id.ToString());
            currentPatient.PatientState = "授权";//病人状态
            string pageText = currentPatient.Sick_Bed_Name + " " + currentPatient.Patient_Name;
            //验证TabControl是否有重复
            for (int i = 0; i < tabControl_Patient.Tabs.Count; i++)
            {
                if (pageText == tabControl_Patient.Tabs[i].Text)
                {
                    tabControl_Patient.SelectedTabIndex = i;
                    return;
                }
            }

            TabControlPanel tabctpnDoc = new TabControlPanel();
            tabctpnDoc.AutoScroll = true;
            TabItem pageDoc = new TabItem();
            pageDoc.Name = currentPatient.Id.ToString();
            pageDoc.Text = pageText;
            pageDoc.Click += new EventHandler(page_Click);
            pageDoc.Tag = currentPatient;
            ucDoctorOperater fm = new ucDoctorOperater(currentPatient, rightDoc_functions);
            fm.Dock = DockStyle.Fill;
            tabctpnDoc.Controls.Add(fm);
            tabctpnDoc.Dock = DockStyle.Fill;
            pageDoc.AttachedControl = tabctpnDoc;
            tabControl_Patient.Controls.Add(tabctpnDoc);
            tabControl_Patient.Tabs.Add(pageDoc);
            tabControl_Patient.Refresh();
            tabControl_Patient.SelectedTab = pageDoc;
        }

        /// <summary>
        /// 取消授权：
        /// 1.删除授权表相关记录。
        /// 2.设置当前病人节点的IsHaveRight属性为false。
        /// 3.设置小卡背景图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 取消授权toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (App.Ask("确定要取消文书授权吗？"))
            {
                if (trvInpatientManager.SelectedNode.Tag != null)
                {
                    InPatientInfo patient_docRight = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                    if (patient_docRight.Sick_Doctor_Id == App.UserAccount.UserInfo.User_id)
                    {
                        string sql_Cancel = "delete from t_set_text_rights where patient_id=" + patient_docRight.Id;
                        int num = App.ExecuteSQL(sql_Cancel);//删除授权表相关记录
                        if (num > 0)
                        {
                            App.Msg("操作成功！");
                            patient_docRight.IsHaveRight = false;//设置当前选中节点的IsHaveRight为false
                            trvInpatientManager.SelectedNode.Tag = patient_docRight;
                            //循环小卡，找到取消授权的病人，把小卡背景图片换成蓝色
                            for (int i = 0; i < ucHospitalIofn1.flowLayoutPanel1.Controls.Count; i++)
                            {
                                UCPictureBox ucp = ucHospitalIofn1.flowLayoutPanel1.Controls[i] as UCPictureBox;
                                InPatientInfo patient_Card = ucp.Tag as InPatientInfo;
                                if (patient_Card.Id == patient_docRight.Id)
                                {
                                    //ucHospitalIofn1.flowLayoutPanel1.Controls[i].BackgroundImage = global::Base_Function.Resource.card_unselect;
                                    break;
                                }
                            }

                        }
                        else
                        {
                            App.Msg("只有管床医生可以取消授权！");
                        }
                    }
                }
            }
        }


        private void trvInpatientManager_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(trvInpatientManager, "");
        }

        /// <summary>
        /// HIS入区操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInArea_Click(object sender, EventArgs e)
        {
            frmImportSouthPatient frmHis = new frmImportSouthPatient();
            frmHis.ShowDialog();
        }

        private void expandableSplitter2_ExpandedChanged(object sender, ExpandedChangeEventArgs e)
        {
            //if (!splitContainer1.Panel2Collapsed)
            //{
            //    splitContainer1.Panel2Collapsed = true;
            //}
            //else
            //{
            //    splitContainer1.Panel2Collapsed = false;
            //}
        }

        private void toolStripMenuItem住院医嘱_Click(object sender, EventArgs e)
        {
            try
            {
                InPatientInfo inPatient = null;
                node = (Node)trvInpatientManager.SelectedNode.DeepCopy();
                if (node != null)
                {
                    inPatient = trvInpatientManager.SelectedNode.Tag as InPatientInfo;
                }
                if (inPatient != null)
                {
                    //CpoeFormMain fc = new CpoeFormMain(inPatient);
                    //App.FormStytleSet(fc, false);
                    //fc.ShowDialog(this);
                    App.MsgWaring("住院医嘱功能尚未启用，目前只提供查询！");
                    frmYZ fc = new frmYZ(inPatient);
                    App.FormStytleSet(fc, false);
                    fc.ShowDialog(this);
                }
            }
            catch
            {
                App.MsgErr("请先选择病人或当前病人没有数据!");
            }
        }

        #region 系统框架调用户控件的事件绑定
        /// <summary>
        /// 查看病程
        /// </summary>
        public void CheckSick(object sender, EventArgs e)
        {
            frmBrowseDoc fm = new frmBrowseDoc();
            App.FormStytleSet(fm, false);
            fm.ShowDialog();
        }

        /// <summary>
        /// 查看体温单
        /// </summary>
        public void CheckTemprature(object sender, EventArgs e)
        {
            frmCheckTemprature fm = new frmCheckTemprature();
            App.FormStytleSet(fm, false);
            fm.ShowDialog();
        }

        /// <summary>
        /// 查看护理记录单
        /// </summary>
        public void CheckNurseRecord(object sender, EventArgs e)
        {
            frmCheckNurseRecord fm = new frmCheckNurseRecord();
            App.FormStytleSet(fm, false);
            fm.ShowDialog();
        }

        /// <summary>
        /// 查看检验检查报告
        /// </summary>
        public void CheckLis(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 查看影像报告
        /// </summary>
        public void CheckPacs(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 手术审批
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CheckOperator(object sender, EventArgs e)
        {
            frmApp fm = new frmApp();
            fm.MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            UcApproval ucApproval = new UcApproval();
            ucApproval.Dock = DockStyle.Fill;
            fm.Text = "手术审批";
            fm.Controls.Add(ucApproval);
            App.FormStytleSet(fm, false);
            fm.ShowDialog();
        }

        /// <summary>
        /// 病案借阅申请
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PatientSickInfoApply(object sender, EventArgs e)
        {
            UcApply_Medical_Record uc = new UcApply_Medical_Record();
            uc.browse_Book += new UcApply_Medical_Record.RefEventHandler(ucapply_browse_Book2);
            uc.Dock = DockStyle.Fill;
            frmApp fm = new frmApp();
            fm.Text = "病案借阅申请";
            fm.Controls.Add(uc);
            App.FormStytleSet(fm, false);
            fm.ShowDialog();
        }

        /// <summary>
        /// 归档退回申请
        /// </summary>
        public void BackSickInfoApply(object sender, EventArgs e)
        {
            UcApply_DocReturn_Record uc = new UcApply_DocReturn_Record();
            uc.Dock = DockStyle.Fill;
            frmApp fm = new frmApp();
            fm.Text = "归档退回申请";
            fm.Controls.Add(uc);
            App.FormStytleSet(fm, false);
            fm.ShowDialog();
        }

        /// <summary>
        /// 运行病历查阅
        /// </summary>
        public void UsedSickInfoCheck(object sender, EventArgs e)
        {
            UCMedicalRightRun_Search uc = new UCMedicalRightRun_Search();
            //uc.browse_Book += new UCMedicalRightRun_Search.RefEventHandler(ucapply_browse_Book2);
            uc.Dock = DockStyle.Fill;
            frmApp fm = new frmApp();
            fm.Text = "运行病例查阅";
            fm.Controls.Add(uc);
            App.FormStytleSet(fm, false);
            fm.ShowDialog();
        }

        /// <summary>
        /// 被授权文书操作
        /// </summary>
        public void DocRights(object sender, EventArgs e)
        {
            UcDocRight uc = new UcDocRight();
            uc.browse_Book += new UcDocRight.RefEventHandler(uCDocRight_browse_Book);
            uc.Dock = DockStyle.Fill;
            frmApp fm = new frmApp();
            fm.Text = "被授权文书操作";
            fm.Controls.Add(uc);
            App.FormStytleSet(fm, false);
            fm.ShowDialog();
        }

        /// <summary>
        /// 病案整理
        /// </summary>
        public void MedicalRecordFinishing(object sender, EventArgs e)
        {
            UcClear uc = new UcClear();
            uc.Dock = DockStyle.Fill;
            frmApp fm = new frmApp();
            fm.Text = "病案整理";
            fm.Controls.Add(uc);
            App.FormStytleSet(fm, false);
            fm.ShowDialog();
        }

        /// <summary>
        /// 病案归档
        /// </summary>
        public void MedicalRecords(object sender, EventArgs e)
        {
            UcArchive uc = new UcArchive();
            uc.Dock = DockStyle.Fill;
            frmApp fm = new frmApp();
            fm.Text = "病案归档";
            fm.Controls.Add(uc);
            //fm.WindowState = FormWindowState.Maximized;
            App.FormStytleSet(fm, false);
            fm.ShowDialog();
        }

        /// <summary>
        /// 知情同意书批处理打印窗体
        /// </summary>
        public void BachePrint(object sender, EventArgs e)
        {
            if (currentPatient!=null)
            {
                frmBachePrints fm = new frmBachePrints(currentPatient);
                fm.ShowDialog(); 
            }
        }

        #endregion
     

        private void 临床路径toolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentPatient != null)
            {
                DataInit.OpenPatientPath(currentPatient.Id);
            }
        }

        private void tabControl_Patient_SelectedTabChanged(object sender, TabStripTabChangedEventArgs e)
        {
            InPatientInfo patient = tabControl_Patient.SelectedTab.Tag as InPatientInfo;
            if (patient != null)
            {
                currentPatient = patient;
                //设置病人状态
                if (currentPatient.PatientState=="借阅")
                {
                    DataInit.boolAgree = true;
                    DataInit.isRightDoc = false;
                }
                else if (currentPatient.PatientState=="授权")
                {
                    DataInit.isRightDoc = true;
                    DataInit.boolAgree = false;
                }
                else
                {
                    DataInit.isRightDoc = false;
                    DataInit.boolAgree = false;
                }

                string sex = null;
                if (patient.Gender_Code.Equals("0") || patient.Gender_Code.Equals("男"))
                {
                    sex = "男";
                }
                else
                {
                    sex = "女";
                }
                App.SetMainFrmMsgToolBarText(patient.Sick_Bed_Name + "床  " + "ID:" + patient.Id.ToString() +
                                            "  住院号:" + patient.PId + "  姓名:" + patient.Patient_Name +
                                            "  性别:" + sex + "  年龄:" + patient.Age.ToString() +
                                            "  入院时间:" + patient.In_Time.ToString() +
                                            "  当前科室:" + patient.Sick_Area_Name);

                //PageSelectChange;

                for (int i = 0; i < tabControl_Patient.SelectedPanel.Controls.Count; i++)
                {
                    if (tabControl_Patient.SelectedPanel.Controls[i].GetType().ToString().Contains("Base_Function.BLL_DOCTOR.ucDoctorOperater"))
                    {
                        ucDoctorOperater temp = (ucDoctorOperater)tabControl_Patient.SelectedPanel.Controls[i];
                        temp.IniMainToobar();
                        temp.PageSelectChange(sender, e);
                    }
                }

            }
            else
            {
                App.A_Commit = null;
                App.A_TempSave = null;
                App.SetToolButtonByUser("ttsbtnPrint", false);
                App.SetToolButtonByUser("ttsbtnPrintContinue", false);
                App.SetToolButtonByUser("tsbtnTempSave", false);
                App.SetToolButtonByUser("tsbtnCommit", false);
                App.SetToolButtonByUser("tsbtnTemplateSave", false);//保存模版
                App.SetToolButtonByUser("btnInsertDiosgin", false);
                App.SetToolButtonByUser("btnRefreshDiosgin", false);
                //App.MainToolBar.Items.Clear();
                //for (int i = 0; i < tempMainToolbar.Items.Count; i++)
                //{
                //    App.MainToolBar.Items.Add(tempMainToolbar.Items[i].Copy());
                //}
                //App.MainToolBar.Refresh();
                //App.MainToolBar.Width = App.MainToolBar.Parent.Width;
            }
        }

        /// <summary>
        /// 未签名文书查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmnitLookBook_Click(object sender, EventArgs e)
        {
            frmLookSignByDoctor frmSign = new frmLookSignByDoctor(currentPatient.Patient_Name);
            App.FormStytleSet(frmSign, false);
            frmSign.ShowDialog();
        }

        /// <summary>
        /// 历史病案查阅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 历史病案查阅toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //frmBookHistory frmbookHistory = new frmBookHistory(currentPatient);
            //frmbookHistory.ShowDialog();
            //return;
            ucHospitalization_Records uc = new ucHospitalization_Records(currentPatient.Patient_Name);
            uc.Dock = DockStyle.Fill;
            frmApp fm = new frmApp();
            fm.Text = "运行病案查阅";
            fm.Controls.Add(uc);
            App.FormStytleSet(fm, false);
            fm.ShowDialog();
        }

        /// <summary>
        /// 特殊科室手动转科
        ///  一、二区内互转
        /// 北院心血管介入科（12病室）与北院心血管CCU科（13病室）两科室病人相互转科，不是转科，需要转入转出记录，
        /// 病案首页也不生成转科科别，即与北院肿瘤一区二区，北院消化一区二区相同。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 手动转区toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string strSectionName = trvInpatientManager.SelectedNode.Parent.Text;
            string strToSectionName = string.Empty;
            if (strSectionName.Contains("一区"))
            {
                strToSectionName = strSectionName.Substring(0, strSectionName.Length - 2) + "二区";
            }
            else if (strSectionName.Contains("二区"))
            {
                strToSectionName = strSectionName.Substring(0, strSectionName.Length - 2) + "一区";
            }
            else if (strSectionName.Contains("北院心血管介入科"))
            {
                strToSectionName = "北院心血管CCU科";
            }
            else if (strSectionName.Contains("北院心血管CCU科"))
            {
                strToSectionName = "北院心血管介入科";
            }
            else
            {
                App.Msg("提示:该科室不是多病区科室!");
                return;
            }
                    
            DialogResult dr = MessageBox.Show( "是否需要将该患者:(" + currentPatient.Patient_Name + ")转到" + strToSectionName,"手动转" + strToSectionName, MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                string strCurSid = string.Empty;
                string strCurSname = string.Empty;
                string strCurSickid = string.Empty;
                string strCurSickname = string.Empty;
                StringBuilder sb = new StringBuilder();
                sb.Append("select a.sid,a.section_name,c.said,c.sick_area_name from t_sectioninfo a,");
                sb.Append(" t_section_area b,t_sickareainfo c where a.sid=b.sid and b.said=c.said");
                sb.Append(" and a.section_name='" + strToSectionName + "'");
                DataSet objDataSet = App.GetDataSet(sb.ToString());
                if (objDataSet.Tables.Count > 0 && objDataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow objDataRow = objDataSet.Tables[0].Rows[0];
                    strCurSid = objDataRow["sid"].ToString();
                    strCurSname = objDataRow["section_name"].ToString();
                    strCurSickid = objDataRow["said"].ToString();
                    strCurSickname = objDataRow["sick_area_name"].ToString();
                }
                if (strCurSname.Length==0)
                {
                    return;
                }

                sb = new StringBuilder("Update t_in_patient Set");
                sb.Append(" section_id=" + strCurSid + ",");
                sb.Append(" section_name='" + strCurSname + "',");
                sb.Append(" sick_area_id=" + strCurSickid + ",");
                sb.Append(" sick_area_name='" + strCurSickname + "'");
                sb.Append(" where id=" + currentPatient.Id);
                try
                {
                    Bifrost.App.ExecuteSQL(sb.ToString());

                    #region 刷新树和卡片
                    RefAllTree();
                    Node objNode1=null;
                    Node objNode2=null;
                    Node objNode3=null;
                    foreach (Node objn in trvInpatientManager.Nodes)
                    {
                        if(objn.Text.Contains("患者管理"))
                        {
                            objNode1 = objn;
                        }
                    }
                    foreach(Node objn in objNode1.Nodes)
                    {
                        if (objn.Text.Contains("科室病人"))
                        {
                            objNode2 = objn;
                        }
                    }
                    foreach (Node objn in objNode2.Nodes)
                    {
                        if (objn.Text.Contains(strSectionName))//(App.UserAccount.CurrentSelectRole.Section_name))
                        {
                            objNode3 = objn;
                        }
                    }
                    if (objNode3 != null)
                    {
                        trvInpatientManager.SelectedNode = objNode3;
                    }
                    #endregion
                    Bifrost.App.Msg("手动转科成功！");
                }
                catch
                {
                    Bifrost.App.MsgErr("手动转科发生异常！");
                }
                finally
                {
                    this.手动转区toolStripMenuItem1.Visible = false;
                }
            }
        }

        /// <summary>
        /// 危急值查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonX1_Click(object sender, EventArgs e)
        {
            //PACS危急值提醒
            string pacsSql = "select a.id,pid 住院号,patient_name 患者姓名,(case when gender_code=0 then '男' else '女' end) 性别,concat(age,age_unit) 年龄,sick_bed_no 床位,section_name 当前科室,vjz 危急值,(case when c.id is null then '未反馈' else '已反馈' end) 反馈状态 from t_pasc_data a inner join t_in_patient b on a.zyh=b.pid left join VJZ_MESSAGE c on a.id=c.id  where vjz is not null and b.leave_time is null and sick_doctor_id= " + App.UserAccount.UserInfo.User_id;
            //LIS危急值提醒
            string lisSql = "select report_item_id,pid 住院号,patient_name 患者姓名,(case when gender_code=0 then '男' else '女' end) 性别,concat(b.age,b.age_unit) 年龄,sick_bed_no 床位,section_name 当前科室,item_name 项目名称,main_result 危急值,unit 单位,result_time 报告时间,(case when c.id is null then '未反馈' else '已反馈' end) 反馈状态 from esb_view_test_report@esb a inner join t_in_patient b on a.inp_no=b.pid left join VJZ_MESSAGE c on a.report_item_id=c.id where a.alarm_flag is not null and b.leave_time is null and sick_doctor_id= " + App.UserAccount.UserInfo.User_id;
            //病理
            string blSql = "select a.medicalno, pid 住院号,patient_name 患者姓名,(case when gender_code=0 then '男' else '女' end) 性别,concat(age,age_unit) 年龄,sick_bed_no 床位,section_name 当前科室,a.SFYX 是否阳性,(case when c.id is null then '未反馈' else '已反馈' end) 反馈状态 from ESB_VIEW_BLZD@esb a inner join t_in_patient b on a.zyh=b.pid left join VJZ_MESSAGE c on a.medicalno=c.id  where a.sfyx is not null and b.leave_time is null and sick_doctor_id= " + App.UserAccount.UserInfo.User_id;

            Class_Table[] tabs = new Class_Table[3];
            tabs[0] = new Class_Table();
            tabs[0].Sql = pacsSql;
            tabs[0].Tablename = "pacs";

            tabs[1] = new Class_Table();
            tabs[1].Sql = lisSql;
            tabs[1].Tablename = "lis";

            tabs[2] = new Class_Table();
            tabs[2].Sql = blSql;
            tabs[2].Tablename = "bl";

            DataSet dsVJZ = App.GetDataSet(tabs);
            if (dsVJZ == null)
            {
                App.Msg("没有找到与危急值相关数据！");
                return;
            }
            FrmVJZ vjz = new FrmVJZ(dsVJZ, false);
            vjz.Show();
        }

        private void 影像查看StripMenuItem_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    //if (currentPatient.His_id != "")
            //    //{
            //    //    XePacsCall(2, currentPatient.His_id, 1);
            //    //}


            //    //XePacsCall(2, "0000243510", 1);
            //    string path = App.SysPath;

            //    Process myprocess = new Process();
            //    ProcessStartInfo startInfo = new ProcessStartInfo(path + @"\pacsForEmr\PacsForEmr.exe");
            //    myprocess.StartInfo = startInfo;
            //    myprocess.StartInfo.UseShellExecute = false;
            //    myprocess.Start();
            //}
            //catch (Exception ex)
            //{

            //}
            try
            {
                

                //if (DataInit.ViewSwitch == 1)
                //{
                //    inpat = DataInit.GetInpatientInfoByPid();//GetPatientByList();
                //}
                if (currentPatient != null)
                {

                    Bifrost.HisInStance.frm_Pasc fc = new Bifrost.HisInStance.frm_Pasc(currentPatient);
                    App.FormStytleSet(fc, false);
                    fc.Show();
                }
            }
            catch
            {
                //App.MsgErr("请先选择病人!");
            }
        }

        /// <summary>
        /// 未完成工作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UnfinishedWork(object sender, EventArgs e)
        {
            frmUnWork frmUW = new frmUnWork();
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Text == frmUW.Text)
                {
                    frm.Visible = true;
                    frm.WindowState = FormWindowState.Normal;
                    frm.Activate();
                    return;
                }
            }
            frmUW.Show();
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            DataSet ds = getLbxx();
            if (ds != null)
            {
               frmElementQuality fm = new frmElementQuality(ds);
                fm.Show();
            }
            else
            {
                //App.Msg("没有找到相关数据！");
            }

        }
        ///<summary>
        ///量表信息
        ///</summary>
        private DataSet getLbxx()
        {
            string sql = "";//where b.section_id= or b.sick_area_id
            if (App.UserAccount.CurrentSelectRole.Role_type == "N")
            {
                sql = "select b.pid 住院号,b.patient_name 姓名,b.sick_bed_no 床号,a.elementtype 类型,a.note 内容 from T_QUALITY_ELEMENT  a inner join t_in_patient b on a.patient_id=b.id and b.sick_area_id='" + App.UserAccount.CurrentSelectRole.Sickarea_Id + "'";
            }
            else
            {
                sql = "select b.pid 住院号,b.patient_name 姓名,b.sick_bed_no 床号,a.elementtype 类型,a.note 内容 from T_QUALITY_ELEMENT  a inner join t_in_patient b on a.patient_id=b.id ";
            }
            DataSet ds = App.GetDataSet(sql);
            return ds;
        }

    }
}
