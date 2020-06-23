using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using C1.Win.C1FlexGrid;
using Bifrost;
using System.Collections;
using Base_Function.BASE_COMMON;

namespace Base_Function.BLL_MEDICAL_RECORD_GRADE
{
    /// <summary>
    /// ���������û��ؼ�
    /// </summary> 
    public partial class ucfrmMainGradeRepart : UserControl
    {
        UserRights userRights = new UserRights();
        public int intMark = 0;
        public string strSectionName = "";
        public string strSectionId = "";
        /// <summary>
        /// �жϵ�½��ɫ����
        /// </summary>
        string strRole_tyep = "";
        public string strText = "1";//ȫԺ�������ͱ�ʶ1
        /// <summary>
        /// ���в�������
        /// </summary>
        public string strYunXingDoc = "";
        /// <summary>
        /// ��ĩ��������
        /// </summary>
        public string strZhongMoDoc = "";

        public string strDOCType = "";
        ///// <summary>
        ///// �鿴��ʷ���ֵ�ѡ��
        ///// </summary>
        //public string strSelectHistoryPF = "";
        ///// <summary>
        ///// ҽ��������ʷ����
        ///// </summary>
        //public string strHistoryDoctor = "";
        ///// <summary>
        ///// ����������ʷ����
        ///// </summary>
        //public string strHistorySeciton = "";
        ///// <summary>
        ///// ȫԺ��ʷ����
        ///// </summary>
        //public string strHistoryHospital = "";
        ///// <summary>
        ///// ��������
        ///// </summary>
        //public string strHistoryDate = "";


        public ucfrmMainGradeRepart()
        {
            InitializeComponent();
        }

        public ucfrmMainGradeRepart(ArrayList buttonRights)
        {
            try
            {
                InitializeComponent();
                SetSickName();
                this.cboxRand.SelectedIndex = 0;
                this.btnQuery.Enabled = false;
                this.btnAddGrade.Enabled = false;
                this.btnSave.Enabled = false;
                this.button5.Enabled = false;
                //��д��Ȩ��
                if (userRights.isExistRole("tsbtnWrite", buttonRights))
                {
                    this.btnAddGrade.Enabled = true;
                    this.btnSave.Enabled = true;
                    this.btnQuery.Enabled = true;
                }
                //�鿴��Ȩ��
                if (userRights.isExistRole("tsbtnLook", buttonRights))
                {
                    this.button5.Enabled = true;
                }
            }
            catch
            {
            }
        }
        DataTable dt = new DataTable();//���ݲ�ѯ������ʱ���һ�ΰ󶨵�datatable
        DataView dv;//Ҫ���������ȡ��ʱ�򱣴����ݵ��Զ�����ͼ
        DataTable dt2;//��������ڰ󶨵�datatable
        //DataRowView drview;
        private void SetSickName()
        {
            string sickSQL = "select a.section_name,a.sid from T_SECTIONINFO a inner join t_section_area  b on a.sid =b.sid order by section_name";
            DataSet ds = App.GetDataSet(sickSQL);
            DataRow dr = ds.Tables[0].NewRow();
            dr["sid"] = "0";
            dr["section_name"] = "ȫԺ";
            ds.Tables[0].Rows.InsertAt(dr, 0);
            this.cboxSick.DataSource = ds.Tables[0].DefaultView;
            this.cboxSick.DisplayMember = "section_name";
            this.cboxSick.ValueMember = "sid";
        }
        /// <summary>
        /// ��ȡ�ܴ�ҽ��
        /// </summary>
        private void GetDoctor()
        {
            string Sql = "select distinct(a.user_id),a.user_name from t_userinfo a" +
                         " inner join t_account_user b on a.user_id=b.user_id" +
                         " inner join t_account c on b.account_id = c.account_id" +
                         " inner join t_acc_role d on d.account_id = c.account_id" +
                         " inner join t_role e on e.role_id = d.role_id" +
                         " inner join t_acc_role_range f on d.id = f.acc_role_id" +
                // " where f.section_id='" + strSectionId + "' and  e.role_type='D' and a.Profession_Card='true'";
                        " where e.role_type='D' and a.Profession_Card='true'";
            DataSet dsuser = App.GetDataSet(Sql);
            if (dsuser != null)
            {
                DataTable dt = dsuser.Tables[0];
                DataRow drnew = dt.NewRow();
                drnew["user_id"] = "0";
                drnew["user_name"] = "";
                dt.Rows.InsertAt(drnew, 0);
                if (dt != null)
                {
                    cbbDoctorr.DisplayMember = "user_name";
                    cbbDoctorr.ValueMember = "user_id";
                    cbbDoctorr.DataSource = dt.DefaultView;
                }
            }
        }
        int selRow = 0;
        private void pingfentoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (c1FlexGrid1.RowSel <= 0)
            {
                App.Msg("����δѡ��Ҫ���ֵ���");
                return;
            }
            if (checkBox2.Checked == true)
            {
                strDOCType = "1";
            }
            else
            {
                strDOCType = "2";
            }
            frmGrade fg = new frmGrade(this, strText, true, strDOCType);
            fg.ShowDialog();
        }
        /// <summary>
        /// ��סԺ�Ŵ���ȥ��PID��
        /// </summary>
        /// <returns></returns>
        public string SetPingfen()
        {
            if (c1FlexGrid1.RowSel >= 0)
            {
                return c1FlexGrid1[c1FlexGrid1.RowSel, "סԺ��"].ToString();
            }
            else
            {
                App.Msg("��ûѡ���˰�");
                return "";
            }
        }

        /// <summary>
        /// �ѻ���ID����ȥ��ID��
        /// </summary>
        /// <returns></returns>
        public string SetPatientID()
        {
            if (c1FlexGrid1.RowSel >= 0)
            {
                return c1FlexGrid1[c1FlexGrid1.RowSel, "���"].ToString();
            }
            else
            {
                App.Msg("��ûѡ���˰�");
                return "";
            }
        }

        /// <summary>
        /// �ѹܴ�ҽ����������ȥ
        /// </summary>
        /// <returns></returns> 
        public string SetSuffererName()
        {
            if (c1FlexGrid1.RowSel >= 0)
            {
                return c1FlexGrid1[c1FlexGrid1.RowSel, "�ܴ�ҽ��"].ToString();
            }
            else
            {
                App.Msg("��ûѡ���˰�");
                return "";
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //�鿴
            if (ucC1FlexGrid1.fg.RowSel <= 0)
            {
                App.Msg("����δѡ��Ҫ�鿴����");
                return;
            }
            frmMainSelectHistoryRepart fsht = new frmMainSelectHistoryRepart(this);
            fsht.ShowDialog();
        }
        /// <summary>
        /// ����ʱ������ֵ�ʱ�䴫��ȥ
        /// </summary>
        /// <returns></returns>
        public string SetTime()
        {
            return ucC1FlexGrid1.fg[ucC1FlexGrid1.fg.RowSel, "����ʱ��"].ToString();
        }
        /// <summary>
        /// ����������ֲ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            frmAddGradeRepart fagr = new frmAddGradeRepart(this);
            App.ButtonStytle(fagr, false);
            fagr.ShowDialog();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            this.ucC1FlexGrid1.fg.DataSource = null;
            string num = this.cboxRand.Text;//��ѯ��������
            string querySQL = "";
            if (DateTime.Compare(DateTime.Parse(dtpInTime1.Value.ToString("yyyy-MM-dd")), DateTime.Parse(dtpIntime2.Value.ToString("yyyy-MM-dd"))) > 0)
            {
                App.Msg("��Ժ������д����");
                return;
            }
            #region ��ĩ����
            if (checkBox2.Checked == true)
            {
                strZhongMoDoc = "1";
                querySQL =
                 " select distinct a.id as ���,a.pid as סԺ��,(select section_name from t_sectioninfo where sid= a.section_id) as ����, a.patient_name as ��������, to_char(a.in_time, 'yyyy-MM-dd HH24:mi') as ��Ժ����, to_char(a.die_time, 'yyyy-MM-dd HH24:mi') as ��Ժ����, a.Sick_Doctor_Name as �ܴ�ҽ��, " +
                 "(select max(b.sum_point) from t_Doc_Grade b where a.pid=b.pid and b.emptype = 'D' and b.alltypepf='1') as ȫԺҽ�Ʒ�ֵ, " +
                    //"(select max(b.sum_point) from t_Doc_Grade b where a.pid=b.pid and b.emptype = 'N' and b.alltypepf='1') as ȫԺ������ֵ," +
                "(select max(b.sum_point)  from t_Doc_Grade b  where a.pid=b.pid and b.emptype = 'D' and b.sectiontypepf='2') as ����ҽ�Ʒ�ֵ," +
                    //" (select max(b.sum_point) from t_Doc_Grade b where a.pid=b.pid and b.emptype = 'N' and b.sectiontypepf='2') as ���һ�����ֵ," +
                " (select max(b.sum_point)  from t_Doc_Grade b  where a.pid=b.pid and b.emptype = 'D' and b.doctortypepf='3') as ҽ��ҽ�Ʒ�ֵ" +
                    //" (select max(b.sum_point) from t_Doc_Grade b where a.pid=b.pid and b.emptype = 'N' and b.doctortypepf='3') as ҽ��������ֵ" +
                " from T_IN_Patient a left join t_Doc_Grade b on a.pid =b.pid where 1=1";


                //                else
                //                {
                //                    querySQL = @"select distinct(c.sum_point) as ��ֵ, 
                //                               a.id as ���,
                //                               b.section_name as ����,
                //                               regexp_substr(a.his_id, '[^-]+', 1, 1) as id,
                //                               a.pid as סԺ��,
                //                               a.patient_name as ��������,
                //                               a.in_time as סԺ����,
                //                               a.die_time as ��Ժ����,
                //                               a.Sick_Doctor_Name as �ܴ�ҽ��
                //                          from T_IN_Patient a, t_sectioninfo b, t_doc_grade c
                //                         where a.section_id = b.sid  and a.pid =c.pid";
                //                }
            }
            #endregion
            #region ���в���
            if (checkBox1.Checked == true)
            {
                strYunXingDoc = "2";
                querySQL =
              " select distinct a.id as ���,a.pid as סԺ��,(select section_name from t_sectioninfo where sid= a.section_id) as ����, a.pid as סԺ��, a.patient_name as ��������, to_char(a.in_time, 'yyyy-MM-dd HH24:mi') as ��Ժ����, to_char(a.die_time, 'yyyy-MM-dd HH24:mi') as ��Ժ����, a.Sick_Doctor_Name as �ܴ�ҽ��, " +
                    // "(100-(select max(b.sum_point) from t_Doc_Grade b where a.pid=b.pid and b.emptype = 'D' and b.alltypepf='1')) as ȫԺҽ�ƿ۷�ֵ, " +
                    //"(100-(select max(b.sum_point) from t_Doc_Grade b where a.pid=b.pid and b.emptype = 'N' and b.alltypepf='1')) as ȫԺ�����۷�ֵ," +
                    //"(100-(select max(b.sum_point)  from t_Doc_Grade b  where a.pid=b.pid and b.emptype = 'D' and b.sectiontypepf='2')) as ����ҽ�ƿ۷�ֵ," +
                    //" (100-(select max(b.sum_point) from t_Doc_Grade b where a.pid=b.pid and b.emptype = 'N' and b.sectiontypepf='2')) as ���һ����۷�ֵ," +
                    //" (100-(select max(b.sum_point)  from t_Doc_Grade b  where a.pid=b.pid and b.emptype = 'D' and b.doctortypepf='3')) as ҽ��ҽ�ƿ۷�ֵ," +
                    //" (100-(select max(b.sum_point) from t_Doc_Grade b where a.pid=b.pid and b.emptype = 'N' and b.doctortypepf='3')) as ҽ�������۷�ֵ" +
              "(select max(b.sum_point) from t_Doc_Grade b where a.pid=b.pid and b.emptype = 'D' and b.alltypepf='1') as ȫԺҽ�Ʒ�ֵ, " +
                "(select max(b.sum_point)  from t_Doc_Grade b  where a.pid=b.pid and b.emptype = 'D' and b.sectiontypepf='2') as ����ҽ�Ʒ�ֵ," +
                " (select max(b.sum_point)  from t_Doc_Grade b  where a.pid=b.pid and b.emptype = 'D' and b.doctortypepf='3') as ҽ��ҽ�Ʒ�ֵ" +
             " from T_IN_Patient a left join t_Doc_Grade b on a.pid =b.pid where a.die_time is null";
            }
            #endregion

            if (this.cboxSick.Text == "ȫԺ")
            {
                if (strZhongMoDoc == "1")
                {
                    querySQL += " and to_char(die_time,'yyyy-MM') like '" + this.dateTimePickerIN_OUT_time.Text + "%'";
                }
            }
            else
            {
                if (strZhongMoDoc == "1")
                {
                    querySQL += " and (select section_name from t_sectioninfo where sid = a.section_id)='" + this.cboxSick.Text + "' and to_char(die_time,'yyyy-MM') like '" + this.dateTimePickerIN_OUT_time.Text + "%'";
                }
                if (strYunXingDoc == "2")
                {
                    querySQL += " and (select section_name from t_sectioninfo where sid = a.section_id)='" + this.cboxSick.Text + "'";

                }
            }
            if (txtName.Text != "")
            {
                querySQL += " and a.patient_name='" + txtName.Text.ToString() + "'";
            }
            if (txtZYH.Text != "")
            {
                querySQL += " and a.pid = '" + txtZYH.Text.ToString() + "'";
            }
            if (this.cbbDoctorr.Text != "")
            {
                querySQL += " and a.Sick_Doctor_Name='" + cbbDoctorr.Text.ToString() + "'";
            }
            if (checkBox7.Checked == true)
            {

                //�����鿴��ʷ����
                if (checkBox5.Checked == true)
                {
                    //querySQL += " and  (select max(b.sum_point) from t_Doc_Grade b where a.pid=b.pid and b.emptype = 'D' and b.alltypepf='1')";//�鿴ȫԺ��ʷ����
                    querySQL += " and (select distinct(b.alltypepf) from t_Doc_Grade b where a.pid = b.pid and b.emptype = 'D' and b.alltypepf = '1') =1";
                }
                if (checkBox4.Checked == true)
                {
                    //querySQL += " and  (select max(b.sum_point) from t_Doc_Grade b where a.pid=b.pid and b.emptype = 'D' and b.sectiontypepf='1')";//�鿴������ʷ����
                    querySQL += " and (select distinct(b.sectiontypepf) from t_Doc_Grade b where a.pid = b.pid and b.emptype = 'D' and b.sectiontypepf = '2') =2";
                }
                if (checkBox3.Checked == true)
                {
                    //querySQL += " and  (select max(b.sum_point) from t_Doc_Grade b where a.pid=b.pid and b.emptype = 'D' and b.doctortypepf='1')";//�鿴ҽ����ʷ����
                    querySQL += " and (select distinct(b.doctortypepf) from t_Doc_Grade b where a.pid = b.pid and b.emptype = 'D' and b.doctortypepf = '3') =3";
                }
                if (checkBox6.Checked == true)
                {
                    querySQL += " and  to_char(b.grade_time,'yyyy-MM')>='" + datePFStart.Text + "' and to_char(b.grade_time,'yyyy-MM')<='" + datePFEnd.Text + "'";//�鿴ҽ����ʷ����
                }

            }
            if (checkBox8.Checked == true)
            {
                querySQL += " and  to_char(a.in_time,'yyyy-MM-DD')>='" + dtpInTime1.Value.ToString("yyyy-MM-dd") + "' and to_char(a.in_time,'yyyy-MM-DD')<='" + dtpIntime2.Value.ToString("yyyy-MM-dd") + "'";
            }
            dt = App.GetDataSet(querySQL).Tables[0];//������Դ��ֵ


            if (this.cboxRand.Text == "��ѡ��")
            {
                //Dataview�Ǳ�ʾ��������ɸѡ���������༭�͵����� System.Data.DataTable �Ŀɰ����ݵ��Զ�����ͼ��
                dv = dt.DefaultView;

                dt2 = dv.ToTable();
            }
            else
            {
                #region ���Ч��

                Random rd = new Random();

                /*���˼· ������Ҫ�����������ȡ����ѭ�����ٴ� ѭ��һ�β���һ�������
                 *Ȼ��ѭ���жϲ����������������û���ظ����� ����оͰ����Ƴ��������²���
                 * ֱ��û���ظ���Ϊֹ��Ȼ���һ���趨��ֵidlist��ֵ idlist��ֵ�����������
                 * ��ÿ�м�¼��IDֵ
                 */
                List<int> randSum = new List<int>();
                randSum.Clear();
                string idlist = "";//����ÿ�м�¼��ID
                DataRow[] dr_list = dt.Select("ȫԺҽ�Ʒ�ֵ is not null or ����ҽ�Ʒ�ֵ is not null or ҽ��ҽ�Ʒ�ֵ is not null");
                foreach (DataRow dr in dr_list)
                {
                    dt.Rows.Remove(dr);
                }
                int dtrowCount = dt.Rows.Count;//��ѯ������
                int randm = Convert.ToInt32(this.cboxRand.Text);//��Ҫ�����ѯ������
                if (dtrowCount < randm)//�����ѯ������С�������ѯ��ѯ����
                {
                    randm = dtrowCount;//��� ��Ҫ�����ѯ������ ���� ��ѯ�����ܵ����� �͸���Ҫ�����ѯ��������ֵ�ɲ�ѯ�ܵ�����
                    //��һ��ѭ���������ȡ����
                    for (int i = 0; i < randm; i++)
                    {
                        if (dt.Rows.Count <= 0)
                        {
                            App.Msg("δ���ҵ�����");
                            c1FlexGrid1.Clear();
                            return;
                        }
                        //����һ�����������datatable����Ҫһ�£������10�� ��Ҫ����0��10֮����������
                        int numSum = rd.Next(0, dt.Rows.Count);
                        //ѭ����������������Ԫ�أ��������ͬ�ľ���ѭ��һ��
                        for (int j = 0; j < randSum.Count; j++)
                        {
                            if (randSum[j] == numSum)
                            {
                                //numSum = rd.Next(0, dt.Rows.Count - 1);

                                //�������ͬ�ľͰ����Ƴ�
                                randSum.RemoveAt(j);
                                i--;//�Ƴ���һ��Ԫ�ؾ�Ҫ��i--��ѭ��һ��
                            }
                        }
                        //ÿ�ΰ�ѭ�������������ӵ�һ����������
                        randSum.Add(numSum);
                        //������ϲ�Ϊ��idlist��ֵ����ID���
                        if (idlist == "")
                        {
                            idlist = dt.Rows[numSum]["���"].ToString();
                        }
                        else
                        {
                            idlist = idlist + "," + dt.Rows[numSum]["���"].ToString();
                        }
                    }
                }
                else
                {
                    //��һ��ѭ���������ȡ����
                    for (int i = 0; i < randm; i++)
                    {
                        if (dt.Rows.Count <= 0)
                        {
                            App.Msg("δ���ҵ�����");
                            c1FlexGrid1.Clear();
                            return;
                        }
                        //����һ�����������datatable����Ҫһ�£������10�� ��Ҫ����0��10֮����������
                        int numSum = rd.Next(0, dt.Rows.Count);
                        //ѭ����������������Ԫ�أ��������ͬ�ľ���ѭ��һ��
                        for (int j = 0; j < randSum.Count; j++)
                        {
                            if (randSum[j] == numSum)
                            {
                                //numSum = rd.Next(0, dt.Rows.Count - 1);

                                //�������ͬ�ľͰ����Ƴ�
                                randSum.RemoveAt(j);
                                i--;//�Ƴ���һ��Ԫ�ؾ�Ҫ��i--��ѭ��һ��
                            }
                        }
                        //ÿ�ΰ�ѭ�������������ӵ�һ����������
                        randSum.Add(numSum);
                        //������ϲ�Ϊ��idlist��ֵ����ID���
                        if (idlist == "")
                        {
                            idlist = dt.Rows[numSum]["���"].ToString();
                        }
                        else
                        {
                            idlist = idlist + "," + dt.Rows[numSum]["���"].ToString();
                        }
                    }
                }
                //���idlist��Ϊ�վ͸�idlist��ֵ
                if (idlist != "")
                {
                    idlist = "��� in (" + idlist + ")";
                }

                #endregion

                //Dataview�Ǳ�ʾ��������ɸѡ���������༭�͵����� System.Data.DataTable �Ŀɰ����ݵ��Զ�����ͼ��
                dv = dt.DefaultView;

                //�ٽ���ɸѡ
                dv.RowFilter = idlist;

                dt2 = dv.ToTable();
            }




            //DataColumn d = new DataColumn("ҽ�Ʒ�ֵ", typeof(double));
            //dt2.Columns.Add(d);

            //DataColumn d1 = new DataColumn("������ֵ", typeof(double));
            //dt2.Columns.Add(d1);

            this.c1FlexGrid1.DataSource = dt2;//������Դ
            this.c1FlexGrid1.Cols["���"].Visible = false;
            //this.c1FlexGrid1.Cols["סԺ��"].Visible = false;
            c1FlexGrid1.Rows[0].TextAlign = TextAlignEnum.CenterCenter;
            //�ı�c1�еĳ���
            for (int i = 1; i < c1FlexGrid1.Cols.Count; i++)
            {
                c1FlexGrid1.Cols[i].Width = 123;
                c1FlexGrid1.Cols[i].StyleNew.TextAlign = TextAlignEnum.CenterCenter;
            }
            this.c1FlexGrid1.Cols["��������"].Width = 50;
            this.c1FlexGrid1.Cols["�ܴ�ҽ��"].Width = 50;
            if (checkBox2.Checked == true)
            {
                this.c1FlexGrid1.Cols["ȫԺҽ�Ʒ�ֵ"].Width = 100;
                //this.c1FlexGrid1.Cols["ȫԺ������ֵ"].Width = 100;
                this.c1FlexGrid1.Cols["ҽ��ҽ�Ʒ�ֵ"].Width = 100;
                //this.c1FlexGrid1.Cols["ҽ��������ֵ"].Width = 100;
                this.c1FlexGrid1.Cols["����ҽ�Ʒ�ֵ"].Width = 100;
                //this.c1FlexGrid1.Cols["���һ�����ֵ"].Width = 100;
            }
            if (checkBox1.Checked == true)
            {
                this.c1FlexGrid1.Cols["ȫԺҽ�Ʒ�ֵ"].Width = 100;
                this.c1FlexGrid1.Cols["ҽ��ҽ�Ʒ�ֵ"].Width = 100;
                this.c1FlexGrid1.Cols["����ҽ�Ʒ�ֵ"].Width = 100;
                //this.c1FlexGrid1.Cols["ȫԺҽ�ƿ۷�ֵ"].Width = 100;
                //this.c1FlexGrid1.Cols["ȫԺ�����۷�ֵ"].Width = 100;
                //this.c1FlexGrid1.Cols["ҽ��ҽ�ƿ۷�ֵ"].Width = 100;
                //this.c1FlexGrid1.Cols["ҽ�������۷�ֵ"].Width = 100;
                //this.c1FlexGrid1.Cols["����ҽ�ƿ۷�ֵ"].Width = 100;
                //this.c1FlexGrid1.Cols["���һ����۷�ֵ"].Width = 100;
            }
            if (checkBox1.Checked == true && checkBox7.Checked == true)
            {
                if (checkBox3.Checked == false)
                {
                    this.c1FlexGrid1.Cols["ҽ��ҽ�Ʒ�ֵ"].Visible = false;
                    //this.c1FlexGrid1.Cols["ҽ�������۷�ֵ"].Visible = false;
                    //this.c1FlexGrid1.Cols["ȫԺҽ�ƿ۷�ֵ"].Visible = false;
                    //this.c1FlexGrid1.Cols["ȫԺ�����۷�ֵ"].Visible = false;
                    //this.c1FlexGrid1.Cols["����ҽ�ƿ۷�ֵ"].Visible = false;
                    //this.c1FlexGrid1.Cols["���һ����۷�ֵ"].Visible = false;
                }
                if (checkBox4.Checked == false)
                {
                    this.c1FlexGrid1.Cols["����ҽ�Ʒ�ֵ"].Visible = false;
                    //this.c1FlexGrid1.Cols["���һ����۷�ֵ"].Visible = false;
                }
                if (checkBox5.Checked == false)
                {
                    this.c1FlexGrid1.Cols["ȫԺҽ�Ʒ�ֵ"].Visible = false;
                    //this.c1FlexGrid1.Cols["ȫԺ�����۷�ֵ"].Visible = false;
                }
            }
            if (checkBox2.Checked == true && checkBox7.Checked == true)
            {
                if (checkBox5.Checked == true)
                {
                    this.c1FlexGrid1.Cols["ҽ��ҽ�Ʒ�ֵ"].Visible = false;
                    //this.c1FlexGrid1.Cols["ҽ��������ֵ"].Visible = false;
                    this.c1FlexGrid1.Cols["����ҽ�Ʒ�ֵ"].Visible = false;
                    //this.c1FlexGrid1.Cols["���һ�����ֵ"].Visible = false;
                }
            }


        }
        /// <summary>
        /// ������ֵ������
        /// </summary>
        /// <param name="values">100-�۷�ֵ</param>
        public void SetFenzhi(double values)
        {
            if (strZhongMoDoc == "1")
            {
                c1FlexGrid1[c1FlexGrid1.RowSel, "ȫԺҽ�Ʒ�ֵ"] = values;
            }
            if (strYunXingDoc == "2")
            {
                c1FlexGrid1[c1FlexGrid1.RowSel, "ȫԺҽ�Ʒ�ֵ"] = values;
            }
        }

        /// <summary>
        /// ������ֵ������(����)
        /// </summary>
        /// <param name="values">100-�۷�ֵ</param>
        public void SetFenzhiNurse(double values)
        {
            //if (strZhongMoDoc == "1")
            //{
            //    c1FlexGrid1[c1FlexGrid1.RowSel, "ȫԺ������ֵ"] = values;
            //}
            //if (strYunXingDoc == "2")
            //{
            //    c1FlexGrid1[c1FlexGrid1.RowSel, "ȫԺ������ֵ"] = values;
            //}
        }


        int setNum = 1;
        /// <summary>
        /// ����ʱ�����¼�
        /// </summary>
        /// <param name="row"></param>
        public void AddRow(Row row)
        {
            if (dt2 != null)
            {
                DataRow dr = dt2.NewRow();//new ��
                //drview = dv.AddNew();
                if (dr.Table.Columns.Count > 0)
                {
                    //��dataRow��ֵ
                    dr[0] = row[2].ToString();
                    dr[1] = row[3].ToString();
                    dr[2] = row[4].ToString();
                    dr[3] = row[5].ToString();
                    dr[4] = Convert.ToDateTime(row[6].ToString());
                    if (row[7].ToString() == "")
                    {
                        dr[5] = DBNull.Value;
                    }
                    else
                    {
                        dr[5] = Convert.ToDateTime(row[7].ToString());
                    }
                    dr[6] = row[8].ToString();

                    //dr[7] = row[9].ToString();
                    //drview["ID"] = row[2].ToString();
                    //drview["����"] = row[3].ToString();
                    //drview["סԺ��"] = row[4].ToString();
                    //drview["��������"] = row[5].ToString();
                    //drview["סԺ����"] = Convert.ToDateTime(row[6].ToString());
                    //if (row[7].ToString() == "")
                    //{
                    //    drview["��Ժ����"] = DBNull.Value;
                    //}
                    //else
                    //{
                    //    drview["��Ժ����"] = Convert.ToDateTime(row[7].ToString());
                    //}
                    //drview["�ܴ�ҽ��"] = row[8].ToString();
                    this.dt2.Rows.Add(dr);
                    this.c1FlexGrid1.DataSource = dt2;
                }
                //else
                //{
                //    //Ϊ�˲�����ÿ��ѭ����Ҫִ������ֻ������ִ��һ��
                //    if (setNum == 1)
                //    {
                //        App.Msg("����֮ǰԭ���ݽṹҪ����");
                //        setNum++;
                //    }
                //}
            }
            else
            {
                if (setNum == 1)
                {
                    App.Msg("����֮ǰԭ���ݽṹҪ����");
                    setNum++;
                }
            }

        }
        int oldRow = 0;
        /// <summary>
        /// ����c1FlexGrid1�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void c1FlexGrid1_Click(object sender, EventArgs e)
        {
            selRow = 1;
            c1FlexGrid1.AllowEditing = false;


            int rows = this.c1FlexGrid1.RowSel;//����ѡ�е��к� 
            if (rows > 0)
            {
                if (oldRow == rows)
                {
                    this.c1FlexGrid1.Rows[rows].StyleNew.BackColor = ColorTranslator.FromHtml("#9BB9ED");
                }
                else
                {
                    //�������ͷ��
                    if (rows > 0)
                    {
                        //�͸ı䱳��ɫ
                        this.c1FlexGrid1.Rows[rows].StyleNew.BackColor = ColorTranslator.FromHtml("#9BB9ED");
                    }
                    if (oldRow > 0 && dt2.Rows.Count >= oldRow)
                    {
                        //������һ�ε�������л�ԭ
                        this.c1FlexGrid1.Rows[oldRow].StyleNew.BackColor = c1FlexGrid1.BackColor;
                    }
                    //else
                    //{
                    //    this.c1FlexGrid1.Rows[rows].StyleNew.BackColor = c1FlexGrid1.BackColor;
                    //}
                }
            }
            //����һ�ε��кŸ�ֵ
            oldRow = rows;
        }
        List<string> itmeList = new List<string>();
        /// <summary>
        ///  ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            //ִ��Ҫ���������sql���
            if (App.ExecuteBatch(itmeList.ToArray()) > 0)
            {
                App.Msg("����ɹ�");
                //ÿ�α���һ�ζ�Ҫ���һ��
                itmeList.Clear();
            }
            else
            {
                App.Msg("����ʧ��");
                itmeList.Clear();
            }

        }
        /// <summary>
        /// ��������list������Ҫ�����������
        /// </summary>
        /// <param name="list"></param>
        public void addPingFen(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                itmeList.Add(list[i]);
            }
        }
        /// <summary>
        /// ������ѯ��ʷ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        DataTable history;
        public void button5_Click(object sender, EventArgs e)
        {
            //��ʷ������ѯʱ��������ʱ����ͬ �ָ��飬���һ����¼
            string selectSQL = "select to_char(grade_time,'yyyy-MM-dd HH24:mi:ss') as ����ʱ��  from t_doc_grade where to_char(grade_time,'yyyy-MM') like '" + dateTimePicker2.Text + "%'group by grade_time ";
            history = App.GetDataSet(selectSQL).Tables[0];
            DataColumn dc = new DataColumn("����", typeof(string));
            dc.DefaultValue = "ȫԺ";
            history.Columns.Add(dc);
            ucC1FlexGrid1.fg.DataSource = history;
            this.ucC1FlexGrid1.fg.Cols["����ʱ��"].Width = 400;
            this.ucC1FlexGrid1.fg.Cols["����"].Width = 400;
        }

        //���û��Ƿ�����UCC1�ؼ��������� �͸�����ֵΪ1
        int rowselect = 0;
        int rowselectold = 0;
        /// <summary>
        /// ������ʷ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ucC1FlexGrid1_Click(object sender, EventArgs e)
        {
            rowselect = 1;
            int rows = this.ucC1FlexGrid1.fg.RowSel;//����ѡ�е��к�
            if (rows > 0)
            {
                if (rowselectold == rows)
                {
                    this.ucC1FlexGrid1.fg.Rows[rows].StyleNew.BackColor = ColorTranslator.FromHtml("#9BB9ED");
                }
                else
                {
                    //�������ͷ��
                    if (rows > 0)
                    {
                        //�͸ı䱳��ɫ
                        this.ucC1FlexGrid1.fg.Rows[rows].StyleNew.BackColor = ColorTranslator.FromHtml("#9BB9ED");
                    }
                    if (rowselectold > 0 && history.Rows.Count >= rowselect)
                    {
                        //������һ�ε�������л�ԭ
                        this.ucC1FlexGrid1.fg.Rows[rowselectold].StyleNew.BackColor = ucC1FlexGrid1.fg.BackColor;
                    }
                }
            }
            //����һ�ε��кŸ�ֵ
            rowselectold = rows;
        }
        private void frmMainGradeRepart_Load(object sender, EventArgs e)
        {
            try
            {
                strRole_tyep = App.UserAccount.CurrentSelectRole.Role_type.ToString();//��ȡ��ǰ��½��ɫ����
                btntransitorily_Save.Visible = false;
                SetSickName();
                this.GetDoctor();
                this.cboxRand.SelectedIndex = 0;
                ucC1FlexGrid1.fg.Click += new EventHandler(ucC1FlexGrid1_Click);
                ucC1FlexGrid1.fg.DoubleClick += new EventHandler(ucC1FlexGrid1_DoubleClick);
                ucC1FlexGrid1.fg.MouseClick += new MouseEventHandler(ucC1FlexGrid1_MouseClick);
                //btnQuery_Click(sender, e);
                ucC1FlexGrid1.fg.ContextMenuStrip = contextMenuStrip1Sel;
                #region ȫԺ����   ���ص�
                //                //�жϵ�ǰ��½����
                //                string strUser_id = App.UserAccount.UserInfo.User_id.ToString();
                //                string strSql = @"select t1.user_id,t1.user_name,t4.role_name
                //                                  from t_userinfo t1,t_account t2, t_account_user t3, t_role t4,t_acc_role t5
                //                                 where t1.user_id = t3.user_id
                //                                   and t3.account_id = t2.account_id
                //                                   and t2.account_id=t5.account_id
                //                                   and t4.role_id=t5.role_id  and t1.user_id='"+strUser_id+"'";
                //                DataSet ds = App.GetDataSet(strSql);
                //                if (ds.Tables[0].Rows.Count > 0)
                //                {
                //                    foreach (DataRow dr in ds.Tables[0].Rows)
                //                    {
                //                        if (dr["role_name"].ToString()== "�����ʿ�ҽʦ")
                //                        {
                //                            intMark = 1;
                //                            strSectionName = App.UserAccount.CurrentSelectRole.Section_name.ToString().Substring(4); ;
                //                            strSectionId = App.UserAccount.CurrentSelectRole.Section_Id.ToString();
                //                            cbbPFType.Enabled = false;//��ֹ�༭��������
                //                            cbbPFType.SelectedItem = "����";
                //                            cboxSick.Text = strSectionName;
                //                            cboxSick.Enabled = false;
                //                            this.GetDoctor();
                //                        }
                //                    }
                //                } 
                #endregion
                if (strRole_tyep == "H")
                {

                }
                //�ж���ʷ�����Ƿ�鿴
                //this.checkBox7.Checked = true;
                //strSelectHistoryPF = "1";//��ʷ�����ж�Ĭ������Ϊ1������Ĭ��ѡ��
                checkBox3.Enabled = false;
                checkBox4.Enabled = false;
                checkBox5.Enabled = false;
                checkBox6.Enabled = false;
                datePFStart.Enabled = false;
                datePFEnd.Enabled = false;
            }
            catch
            {
            }
        }

        private void btntransitorily_Save_Click(object sender, EventArgs e)
        {
            frmMainSelectHistoryRepart fsht = new frmMainSelectHistoryRepart(this);
            fsht.ShowDialog();
            btnQuery_Click(sender, e);
        }
        /// <summary>
        /// ˫�����ɱ���ʱ
        /// </summary>
        int oldRow2 = 0;
        private void c1FlexGrid1_DoubleClick(object sender, EventArgs e)
        {
            c1FlexGrid1.AllowEditing = false;

            int rows = this.c1FlexGrid1.RowSel;//����ѡ�е��к�
            if (rows > 0)
            {
                if (oldRow2 == rows)
                {
                    this.c1FlexGrid1.Rows[rows].StyleNew.BackColor = ColorTranslator.FromHtml("#9BB9ED");
                }
                else
                {
                    //�������ͷ��
                    if (rows > 0)
                    {
                        //�͸ı䱳��ɫ
                        this.c1FlexGrid1.Rows[rows].StyleNew.BackColor = ColorTranslator.FromHtml("#9BB9ED");
                    }
                    if (oldRow2 > 0 && dt2.Rows.Count >= oldRow)
                    {
                        //������һ�ε�������л�ԭ
                        this.c1FlexGrid1.Rows[oldRow2].StyleNew.BackColor = c1FlexGrid1.BackColor;
                    }
                }
            }
            //����һ�ε��кŸ�ֵ
            oldRow2 = rows;
        }
        /// <summary>
        /// ˫����ʷ ����
        /// </summary>
        int hostoryRow = 0;
        private void ucC1FlexGrid1_DoubleClick(object sender, EventArgs e)
        {
            ucC1FlexGrid1.fg.AllowEditing = false;

            int rows = this.ucC1FlexGrid1.fg.RowSel;//����ѡ�е��к�
            if (rows > 0)
            {
                if (hostoryRow == rows)
                {
                    this.ucC1FlexGrid1.fg.Rows[rows].StyleNew.BackColor = ColorTranslator.FromHtml("#9BB9ED");
                }
                else
                {
                    //�������ͷ��
                    if (rows > 0)
                    {
                        //�͸ı䱳��ɫ
                        this.ucC1FlexGrid1.fg.Rows[rows].StyleNew.BackColor = ColorTranslator.FromHtml("#9BB9ED");
                    }
                    if (hostoryRow > 0 && history.Rows.Count >= hostoryRow)
                    {
                        //������һ�ε�������л�ԭ
                        this.ucC1FlexGrid1.fg.Rows[hostoryRow].StyleNew.BackColor = ucC1FlexGrid1.fg.BackColor;
                    }
                }
            }
            //����һ�ε��кŸ�ֵ
            hostoryRow = rows;
        }
        /// <summary>
        /// ���ɱ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void c1FlexGrid1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = c1FlexGrid1.PointToClient(Cursor.Position);

                if (c1FlexGrid1.HitTest(e.X, e.Y).Row >= 1)//���Ƿ�����Ϣ������
                {
                    ctmnspDelete.Show(c1FlexGrid1, p);
                    if (strZhongMoDoc == "1")
                    {
                        if (string.IsNullOrEmpty(c1FlexGrid1[c1FlexGrid1.RowSel, "ȫԺҽ�Ʒ�ֵ"].ToString()))
                        {
                            pingfentoolStripMenuItem1.Enabled = true;
                        }
                        else
                        {
                            pingfentoolStripMenuItem1.Enabled = false;
                        }

                        //if (string.IsNullOrEmpty(c1FlexGrid1[c1FlexGrid1.RowSel, "ȫԺ������ֵ"].ToString()))
                        //{
                        //    pingfenNurseMenuItem.Enabled = true;
                        //}
                        //else
                        //{
                        //    pingfenNurseMenuItem.Enabled = false;
                        //}
                    }
                    if (strYunXingDoc == "2")
                    {
                        if (string.IsNullOrEmpty(c1FlexGrid1[c1FlexGrid1.RowSel, "ȫԺҽ�Ʒ�ֵ"].ToString()))
                        {
                            pingfentoolStripMenuItem1.Enabled = true;
                        }
                        else
                        {
                            pingfentoolStripMenuItem1.Enabled = false;
                        }

                        //if (string.IsNullOrEmpty(c1FlexGrid1[c1FlexGrid1.RowSel, "ȫԺ�����۷�ֵ"].ToString()))
                        //{
                        //    pingfenNurseMenuItem.Enabled = true;
                        //}
                        //else
                        //{
                        //    pingfenNurseMenuItem.Enabled = false;
                        //}
                    }
                }
            }
        }
        /// <summary>
        /// ��ʷ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ucC1FlexGrid1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = ucC1FlexGrid1.fg.PointToClient(Cursor.Position);
                if (ucC1FlexGrid1.fg.HitTest(e.X, e.Y).Row >= 1)
                {
                    contextMenuStrip1Sel.Show(ucC1FlexGrid1, p);
                }
            }
        }

        private void pingfenNurseMenuItem_Click(object sender, EventArgs e)
        {
            if (c1FlexGrid1.RowSel <= 0)
            {
                App.Msg("����δѡ��Ҫ���ֵ���");
                return;
            }
            frmGradeNurse fg = new frmGradeNurse(this);
            fg.ShowDialog();
        }
        /// <summary>
        /// ���������������--Ԭ��130912����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSCSJBB_Click(object sender, EventArgs e)
        {
            string strMark = "";
            if (strZhongMoDoc != "")
            {
                strMark = strZhongMoDoc;
            }
            if (strYunXingDoc != "")
            {
                strMark = strYunXingDoc;
            }
            int blType;
            int lsflag;
            int ysflag;
            int ksflag;
            int qyflag;

            if (checkBox1.Checked == true)
            {
                blType = 1;
            }
            else
            {
                blType = 2;
            }

            if (checkBox7.Checked == true)
            {
                lsflag = 1;
            }
            else
            {
                lsflag = 0;
            }
            if (checkBox3.Checked == true)
            {
                ysflag = 1;
            }
            else
            {
                ysflag = 0;
            }
            if (checkBox4.Checked == true)
            {
                ksflag = 1;
            }
            else
            {
                ksflag = 0;
            }
            if (checkBox5.Checked == true)
            {
                qyflag = 1;
            }
            else
            {
                qyflag = 0;
            }
            frmStochasticRepart frm = new frmStochasticRepart(dt2, strMark, blType, ysflag, ksflag, qyflag, lsflag);
            frm.ShowDialog();




        }
        /// <summary>
        /// �ܴ�ҽ�����ſ��ұ仯���仯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboxSick_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string strSection_Name = cboxSick.Text.ToString();

                if (strSection_Name != "")
                {
                    string strSectionSql = "select t.sid from t_sectioninfo t where t.section_name='" + strSection_Name + "'";
                    DataSet ds = App.GetDataSet(strSectionSql);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string strSection_id = ds.Tables[0].Rows[0]["sid"].ToString();
                        if (strSection_id != "")
                        {
                            string Sql = "select distinct(a.user_id),a.user_name from t_userinfo a" +
                                 " inner join t_account_user b on a.user_id=b.user_id" +
                                 " inner join t_account c on b.account_id = c.account_id" +
                                 " inner join t_acc_role d on d.account_id = c.account_id" +
                                 " inner join t_role e on e.role_id = d.role_id" +
                                 " inner join t_acc_role_range f on d.id = f.acc_role_id" +
                                 " where f.section_id='" + strSection_id + "' and  e.role_type='D' and a.Profession_Card='true'";
                            DataSet dsuser = App.GetDataSet(Sql);
                            if (dsuser != null)
                            {
                                DataTable dt = dsuser.Tables[0];
                                DataRow drnew = dt.NewRow();
                                drnew["user_id"] = "0";
                                drnew["user_name"] = "";
                                dt.Rows.InsertAt(drnew, 0);
                                if (dt != null)
                                {
                                    cbbDoctorr.DisplayMember = "user_name";
                                    cbbDoctorr.ValueMember = "user_id";
                                    cbbDoctorr.DataSource = dt.DefaultView;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }
        /// <summary>
        /// �ж��Ƿ������鿴��ʷ���ֹ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox7_Click(object sender, EventArgs e)
        {
            if (checkBox7.Checked == true)
            {
                if (checkBox1.Checked == true)
                {
                    checkBox3.Enabled = true;
                    checkBox4.Enabled = true;
                }
                else
                {
                    checkBox3.Enabled = false;
                    checkBox4.Enabled = false;
                }
                checkBox5.Enabled = true;
                checkBox6.Enabled = true;
                datePFStart.Enabled = true;
                datePFEnd.Enabled = true;
            }
            else
            {
                checkBox3.Enabled = false;
                checkBox4.Enabled = false;
                checkBox5.Enabled = false;
                checkBox6.Enabled = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;
                datePFStart.Enabled = false;
                datePFEnd.Enabled = false;
            }
        }
        /// <summary>
        /// ҽ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox3_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (checkBox3.Checked == true)
            //    {
            //        checkBox4.Checked = false;
            //        //checkBox4.Checked = false;
            //        checkBox5.Checked = false;
            //        //checkBox5.Checked = false;
            //    }
            //}
            //catch 
            //{

            //}
        }
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox4_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (checkBox4.Checked == true)
            //    {
            //        checkBox3.Checked = false;
            //        //checkBox3.Enabled = false;
            //        checkBox5.Checked = false;
            //        //checkBox5.Enabled = false;
            //    }
            //}
            //catch 
            //{

            //}
        }
        /// <summary>
        /// ȫԺ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox5_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (checkBox5.Checked == true)
            //    {
            //        checkBox3.Checked =false;
            //        //checkBox3.Enabled = false;
            //        checkBox4.Checked = false;
            //        //checkBox4.Enabled = false;
            //    }
            //}
            //catch 
            //{

            //}
        }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox6_Click(object sender, EventArgs e)
        {
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkBox2.Checked = false;
                dateTimePickerIN_OUT_time.Enabled = false;
                strYunXingDoc = "";
                strZhongMoDoc = "";
            }
            if (checkBox1.Checked == false)
            {
                checkBox2.Checked = true;
                dateTimePickerIN_OUT_time.Enabled = true;
                strYunXingDoc = "";
                strZhongMoDoc = "";
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                checkBox1.Checked = false;
                checkBox3.Enabled = false;
                checkBox4.Enabled = false;
                dateTimePickerIN_OUT_time.Enabled = true;
                strYunXingDoc = "";
                strZhongMoDoc = "";
            }
            if (checkBox2.Checked == false)
            {
                checkBox1.Checked = true;
                if (checkBox7.Checked == true)
                {
                    checkBox3.Enabled = true;
                    checkBox4.Enabled = true;
                }
                dateTimePickerIN_OUT_time.Enabled = false;
                strYunXingDoc = "";
                strZhongMoDoc = "";
            }
        }

        private void tsmlsws_Click(object sender, EventArgs e)
        {
            if (c1FlexGrid1.RowSel <= 0)
            {
                App.Msg("����δѡ��Ҫ���ֵ���");
                return;
            }
            if (checkBox2.Checked == true)
            {
                strDOCType = "1";
            }
            else
            {
                strDOCType = "2";
            }
            frmGrade fg = new frmGrade(this, strText, false, strDOCType);
            fg.ShowDialog();
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked == true)
            {
                dtpInTime1.Enabled = true;
                dtpIntime2.Enabled = true;
            }
            else
            {
                dtpInTime1.Enabled = false;
                dtpIntime2.Enabled = false;
            }
        }

        private void btnKFXX_Click(object sender, EventArgs e)
        {
            string patient_id = "";
            int j = 0;
            foreach (DataRow dr in dt2.Rows)
            {
                if (j == 1000)
                {
                    App.Msg("��ѯ��������,���β�ѯ���ܳ���1000�ˡ�");
                    return;
                }
                if (patient_id == "")
                {
                    patient_id = "'" + dr["���"].ToString() + "'";
                }
                else
                {
                    patient_id += ",'" + dr["���"].ToString() + "'";
                }
                j++;
            }
            frmKFXQ kfxq = new frmKFXQ(patient_id);
            kfxq.ShowDialog();
        }
    }
}