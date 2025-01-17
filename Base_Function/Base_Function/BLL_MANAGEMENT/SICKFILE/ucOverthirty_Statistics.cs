﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Bifrost;
using Base_Function.BLL_DOCTOR;
using Base_Function.BASE_COMMON;
using C1.Win.C1FlexGrid;

namespace Base_Function.BLL_MANAGEMENT.SICKFILE
{
    public partial class ucOverthirty_Statistics : UserControl
    {
        public ucOverthirty_Statistics()
        {
            InitializeComponent();
            this.Load += new EventHandler(ucOverthirty_Statistics_Load);
        }

        void ucOverthirty_Statistics_Load(object sender, EventArgs e)
        {
            InitDept();
        }

        /// <summary>
        /// 初始化科室下拉列表
        /// </summary>
        private void InitDept()
        {
            try
            {
                string sql_Section = @"select a.sid,a.section_name,a.section_code from t_sectioninfo a 
                                        inner join t_section_area b on a.sid=b.sid
                                        group  by a.shid,a.sid,a.section_code,a.section_name
                                        order by a.shid,to_number(a.section_code)";//查询科室

                DataSet ds_InSection = new DataSet();

                ds_InSection = App.GetDataSet(sql_Section);
                //插入默认选项（请选择）
                if (ds_InSection != null)
                {
                    DataRow dr = ds_InSection.Tables[0].NewRow();
                    dr["sid"] = 0;
                    dr["section_name"] = "-请选择-";
                    ds_InSection.Tables[0].Rows.InsertAt(dr, 0);

                    //DataRow dr1 = ds_InSection.Tables[0].NewRow();
                    //dr1["sid"] = 1;
                    //dr1["section_name"] = "全院";
                    //ds_InSection.Tables[0].Rows.InsertAt(dr1, 1);
                }
                cboDept.DataSource = ds_InSection.Tables[0];
                cboDept.DisplayMember = "section_name";
                cboDept.ValueMember = "sid";
            }
            catch { }
        }

        private void btnStatistics_Click(object sender, EventArgs e)
        {
            Get_Search_Data();
        }

        private void Get_Search_Data()
        {
            string sql = @"select rownum as 序号,b.section_name as 科室,a.上报人,b.patient_name as 患者姓名,b.pid as 住院号,b.in_time as 住院时间,a.上报时间 as 书写时间,a.住院天数,b.sick_doctor_name as 主管医生 from t_overthirty_up a inner join t_in_patient b on a.patient_id = b.id";

            if (this.cboDept.SelectedValue.ToString() == "1")
            {
                sql += " where 1=1";
            }
            else
            {
                sql += " where b.section_id='" + this.cboDept.SelectedValue.ToString() + "'";
            }

            if (this.rbnIntime.Checked)
            {
                string dataStart = dtpInStart.Value.ToString("yyyy-MM-dd ");
                string dataend = dtpInEnd.Value.ToString("yyyy-MM-dd ");

                sql += " and to_char(b.in_time,'yyyy-MM-dd') between '" + dataStart.Trim() + "' and '" + dataend.Trim() + "'";
            }
            else if (this.rbnOuttime.Checked)
            {
                string dataStart = dtpOutStart.Value.ToString("yyyy-MM-dd ");
                string dataend = dtpOutEnd.Value.ToString("yyyy-MM-dd ");

                sql += " and to_char(b.die_time,'yyyy-MM-dd') between '" + dataStart.Trim() + "' and '" + dataend.Trim() + "'";
            }
            else if (this.rbnWritetime.Checked)
            {
                string dataStart = dtpWriteStart.Value.ToString("yyyy-MM-dd ");
                string dataend = dtpWriteEnd.Value.ToString("yyyy-MM-dd ");

                sql += " and a.上报时间  between '" + dataStart.Trim() + "' and '" + dataend.Trim() + "'";
            }
            else if (this.rbnPid.Checked)
            {
                string pid = this.txtPid.Text;

                sql += " and b.pid='" + pid + "'";
            }
            else if (this.rbnName.Checked)
            {
                string name = this.txtName.Text;
                sql += " and b.patient_name='" + name + "'";
            }


            DataSet ds = new DataSet();
            ds = App.GetDataSet(sql);

            if (ds.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("未找到符合该条件的记录！", "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                flgview.Clear();
            }
            else
            {
                this.flgview.DataSource = ds.Tables[0].DefaultView;
                flgview.Rows[0].TextAlign = TextAlignEnum.CenterCenter;
            }

        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "住院超过30天统计表.xls";
            saveFileDialog1.Filter = "Excel工作簿(*.xls)|*.xls";
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string pathname = saveFileDialog1.FileName;
            this.flgview.SaveGrid(pathname, C1.Win.C1FlexGrid.FileFormatEnum.Excel, C1.Win.C1FlexGrid.FileFlags.IncludeFixedCells);
        }

        private void flgview_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (this.flgview.RowSel >= 1)
                {
                    string patient_id = this.flgview[this.flgview.RowSel, "住院号"].ToString();

                    string sql = "select * from t_in_patient t where t.pid='" + patient_id + "'";
                    DataSet ds1 = App.GetDataSet(sql);
                    if (ds1 != null)
                    {
                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            InPatientInfo patientInfo = DataInit.GetIninpatientByPid(ds1.Tables[0]);
                            ucDoctorOperater fq = new ucDoctorOperater(patientInfo);
                            App.UsControlStyle(fq);
                            App.AddNewBusUcControl(fq, "病人文书");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.MsgErr(ex.Message);
            }
        }
    }
}
