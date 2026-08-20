﻿using AljazeeraCPanel;
using AljazeeraCPanel.Models;
using cpanel.Models;
using FCBCPanel.Models;
using iTextSharp.text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Security;
using System.Web.Util;

namespace SIBCPanel.Context
{
    public class DataSource
    {
        //ConnectionString....
        private string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
       
        //-----------------------GET chq------------------------------------------------------
        //
        public int updatechqsts(int id, string sts)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand(
                    "UPDATE jsb_user_service_reqs SET req_status = :sts WHERE req_id = :id", con);
                cmd.Parameters.Add("sts", OracleType.VarChar).Value = sts;
                cmd.Parameters.Add("id", OracleType.Int32).Value = id;

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        public int updatecardsts(int id, string sts)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand(
                    "UPDATE jsb_user_service_reqs SET req_status = :sts WHERE req_id = :id", con);
                cmd.Parameters.Add("sts", OracleType.VarChar).Value = sts;
                cmd.Parameters.Add("id", OracleType.Int32).Value = id;

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        public int deleteprofile(int roleid)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("DELETE FROM tbl_rolemaster WHERE roleid = :roleid", con);
                cmd.Parameters.Add("roleid", OracleType.Int32).Value = roleid;

                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        public int getprofileuserscount(int roleid)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("SELECT count(user_log) AS userscount FROM users WHERE roleid = :roleid", con);
                cmd.Parameters.Add("roleid", OracleType.Int32).Value = roleid;

                int count = 0;
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    OracleDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        count = int.Parse(dr[0].ToString());
                    }
                }
                return count;
            }
        }

        public int getuserscount()
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select count(user_log) as userscount from users_jsb ", con);
                int count = 0;
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    OracleDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        count = int.Parse(dr[0].ToString());
                    }
                }
                return count;
            }
        }

        public int updatecard(int id, string sts)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand(
                    "UPDATE card_reqs SET req_status = :sts WHERE request_id = :id", con);
                cmd.Parameters.Add("sts", OracleType.VarChar).Value = sts;
                cmd.Parameters.Add("id", OracleType.Int32).Value = id;

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        public int Updateinfo(string user, string p)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand(
                    "UPDATE jsb_security_master SET user_pwd = :pwd WHERE user_log = :userlog", con);
                cmd.Parameters.Add("pwd", OracleType.VarChar).Value = p;
                cmd.Parameters.Add("userlog", OracleType.VarChar).Value = user;

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        public Boolean refreshcustomer(int userid, string email, string address, string phonenumber)
        {
            Boolean response = false;
            using (OracleConnection con = new OracleConnection(conString))
            {
                // WAPT01-04: Parameterized — no string concatenation
                OracleCommand cmd = new OracleCommand(
                    "UPDATE users_jsb SET user_mobile = :phonenumber, user_email = :email, user_address = :address WHERE user_id = :userid", con);
                cmd.Parameters.Add("phonenumber", OracleType.VarChar).Value = phonenumber;
                cmd.Parameters.Add("email", OracleType.VarChar).Value = email;
                cmd.Parameters.Add("address", OracleType.VarChar).Value = address;
                cmd.Parameters.Add("userid", OracleType.Int32).Value = userid;

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                int result = cmd.ExecuteNonQuery();
                response = result != -1;
                con.Close();
            }
            return response;
        }
        public Boolean UpdatecustomerSts(string userid, string status)
        {
            Boolean response = false;
            using (OracleConnection con = new OracleConnection(conString))
            {
                // WAPT01-02: Parameterized — no string concatenation
                OracleCommand cmd = new OracleCommand(
                    "UPDATE users_jsb SET user_status = :status WHERE user_log = :userid", con);
                cmd.Parameters.Add("status", OracleType.VarChar).Value = status;
                cmd.Parameters.Add("userid", OracleType.VarChar).Value = userid;

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                int result = cmd.ExecuteNonQuery();
                response = result != -1;
                con.Close();
            }
            return response;
        }

        public EPortReceipt GetEPortReceipt(string portsnotice)
        {
            EPortReceipt receipt = new EPortReceipt();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("SELECT * FROM EPORT WHERE tran_payserviceid = :portsnotice", con);
                cmd.Parameters.Add("portsnotice", OracleType.VarChar).Value = portsnotice;

                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    receipt.tran_id = dr["tranid"].ToString();
                    receipt.tran_paycustomercode = dr["tran_paycustomercode"].ToString();
                    receipt.tran_payserviceid = dr["tran_payserviceid"].ToString();
                    receipt.tran_bankode = dr["tran_bankode"].ToString();
                    receipt.tran_amount = dr["tran_amount"].ToString();
                    receipt.tran_customername = dr["tran_customername"].ToString();
                    receipt.tran_eportresponse = dr["tran_eportresponse"].ToString();
                    receipt.tran_plcno = dr["tran_plcno"].ToString();
                    receipt.tran_curr = dr["tran_curr"].ToString();
                    receipt.tran_bankvoucher = dr["tran_bankvoucher"].ToString();
                    receipt.tran_service = dr["tran_service"].ToString();
                }
                else
                {
                    dr.Close();
                }
                dr.Close();
                con.Close();
            }
            return receipt;
        }

        public List<CustomerTransferReportViewModel> GetAllTransactions()
        {
            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select * from trans_log", con);
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranFullReq = dr["tran_req"].ToString(),
                        TranFullResp = dr["tran_resp"].ToString(),
                        TranDate = dr["tran_resp_date"].ToString(),
                        TranStatus = dr["tran_status"].ToString(),
                        TranResult = dr["tran_resp_result"].ToString(),
                        TranReqAmount = dr["tran_amount"].ToString(),
                        AccountType = dr["tran_name"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public CustomerTransferReportViewModel GetTransactionDetails(string tran_id)
        {
            CustomerTransferReportViewModel transaction = new CustomerTransferReportViewModel();
            using (OracleConnection connection = new OracleConnection(conString))
            {
                OracleCommand command = new OracleCommand(
                    "SELECT * FROM trans_log INNER JOIN users ON users.user_id = trans_log.user_id WHERE tran_id = :tran_id", connection);
                command.Parameters.Add("tran_id", OracleType.VarChar).Value = tran_id;

                connection.Open();
                OracleDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    transaction.TranID = dr["tran_id"].ToString();
                    transaction.TranFullReq = dr["tran_req"].ToString();
                    transaction.TranFullResp = dr["tran_resp"].ToString();
                    transaction.TranDate = dr["tran_resp_date"].ToString();
                    transaction.TranStatus = dr["tran_status"].ToString();
                    transaction.TranResult = dr["tran_resp_result"].ToString();
                    transaction.TranAmount = dr["tran_amount"].ToString();
                    transaction.TranName = dr["tran_name"].ToString();
                    transaction.CustomerName = dr["user_name"].ToString();
                    transaction.User_log = dr["user_log"].ToString();
                }
                dr.Close();
                connection.Close();
            }
            return transaction;
        }

        public List<CustomerTransferReportViewModel> GetBillersReport()
        {
            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select billers_payment_log.bbl_id,billers_payment_log.bbl_trandate,billers_statuses.bil_name,BILLERS_PAYMENT_LOG.bbl_billervoucher,BILLERS_PAYMENT_LOG.bbl_billamount,BILLERS_PAYMENT_LOG.bbl_bnkresponse,BILLERS_PAYMENT_LOG.bbl_reversalstatus,BILLERS_PAYMENT_LOG.bbl_sys_traceno from billers_payment_log inner join BILLERS_STATUSES on BILLERS_PAYMENT_LOG.bbl_billerid = BILLERS_STATUSES.bil_billerid", con);
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        bbl_id = dr["bbl_id"].ToString(),
                        bbl_trandate = dr["bbl_trandate"].ToString(),
                        bil_name = dr["bil_name"].ToString(),
                        bbl_billervoucher = dr["bbl_billervoucher"].ToString(),
                        bbl_billamount = dr["bbl_billamount"].ToString(),
                        bbl_bnkresponse = dr["bbl_bnkresponse"].ToString(),
                        bbl_reversalstatus = dr["bbl_reversalstatus"].ToString(),
                        bbl_sys_traceno = dr["bbl_sys_traceno"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public List<req_res_model> getreq_res_log()
        {
            List<req_res_model> transactions = new List<req_res_model>();

            using (OracleConnection con = new OracleConnection(conString))
            {
               // OracleCommand cmd = new OracleCommand("select * from log_req_res where  RESPONSE_DATA <> 'null' and ( request_data like '%pp%' or request_data like '%ps%' or request_data like '%PP%' or request_data like '%PS%') order by request_date desc", con);
                OracleCommand cmd = new OracleCommand("select * from billers_payment_log inner join billers_statuses on billers_payment_log.BBL_BILLERID = billers_statuses.bil_billerid order by bbl_trandate desc", con);

                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                //String bnkRefernce;
                while (dr.Read())
                {

                    //dynamic jsonRequesrt = JsonConvert.DeserializeObject(dr["bbl_bnkrefrence"].ToString());
                    //bnkRefernce = jsonRequesrt["reference"];

                    transactions.Add(new req_res_model
                    {
                        //ID = dr["ID"].ToString(),
                        //Request_Data = dr["REQUEST_DATA"].ToString(),
                        //Response_Data = dr["RESPONSE_DATA"].ToString(),
                        //CONNECTION_RESPONSE = dr["CONNECTION_RESPONSE"].ToString(),
                        //RESPONSE_DATE = dr["RESPONSE_DATE"].ToString()




                        ID = dr["BBL_ID"].ToString(),
                        TRAN_Data = dr["BBL_TRANDATE"].ToString(),
                        Biller_Name = dr["BIL_NAME"].ToString(),
                        BILLER_VOUCHER = dr["BBL_BILLERVOUCHER"].ToString(),
                        BILL_AMOUNT = dr["BBL_BILLAMOUNT"].ToString(),
                        BBL_BILLERRESPONSE = dr["BBL_BILLERRESPONSE"].ToString(),
                        BBL_BNKREFRENCE = dr["BBL_BNKREFRENCE"].ToString(),
                        //BBL_BNKREFRENCE = bnkRefernce.ToString(),
                        BBL_SYS_TRACENO = dr["BBL_SYS_TRACENO"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public List<req_res_model> getfilteredreq_res_log(string fromdate, string todate, string biller)
        {
            string sqlbiller = "";

            List<req_res_model> transactions = new List<req_res_model>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                //OracleCommand cmd = new OracleCommand("select * from ( select * from log_req_res where to_date(substr(request_date,0,9),'dd-mon-yy') >= to_date('" + fromdate + "','yyyy-mm-dd') and to_date(substr(request_date,0,9),'dd-mon-yy') <= to_date('" + todate + "','yyyy-mm-dd') and RESPONSE_DATA <> 'null' order by request_date desc ) where request_data like '%pp%' or request_data like '%ps%' or request_data like '%PP%' or request_data like '%PS%'", con);
                OracleCommand cmd = new OracleCommand("select * from billers_payment_log where BBL_BILLERID = :biller and to_date(substr(bbl_trandate,0,9),'dd-mon-yy') >= to_date(:fromdate,'yyyy-mm-dd') and to_date(substr(bbl_trandate,0,9),'dd-mon-yy') <= to_date(:todate,'yyyy-mm-dd') order by bbl_trandate desc", con);
                cmd.Parameters.Add("biller", OracleType.VarChar).Value = biller;
                cmd.Parameters.Add("fromdate", OracleType.VarChar).Value = fromdate;
                cmd.Parameters.Add("todate", OracleType.VarChar).Value = todate;

                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                //String bnkRefernce;
                while (dr.Read())
                {

                    //dynamic jsonRequesrt = JsonConvert.DeserializeObject(dr["bbl_bnkrefrence"].ToString());
                    //bnkRefernce = jsonRequesrt["reference"];

                    transactions.Add(new req_res_model
                    {
                        //ID = dr["ID"].ToString(),
                        //Request_Data = dr["REQUEST_DATA"].ToString(),
                        //Response_Data = dr["RESPONSE_DATA"].ToString(),
                        //CONNECTION_RESPONSE = dr["CONNECTION_RESPONSE"].ToString(),
                        //RESPONSE_DATE = dr["RESPONSE_DATE"].ToString()



                        ID = dr["BBL_ID"].ToString(),
                        TRAN_Data = dr["BBL_TRANDATE"].ToString(),
                        Biller_Name = dr["BIL_NAME"].ToString(),
                        BILLER_VOUCHER = dr["BBL_BILLERVOUCHER"].ToString(),
                        BILL_AMOUNT = dr["BBL_BILLAMOUNT"].ToString(),
                        BBL_BILLERRESPONSE = dr["BBL_BILLERRESPONSE"].ToString(),
                        BBL_BNKREFRENCE = dr["BBL_BNKREFRENCE"].ToString(),
                        // BBL_BNKREFRENCE = bnkRefernce.ToString(),
                        BBL_SYS_TRACENO = dr["BBL_SYS_TRACENO"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public List<CustomerTransferReportViewModel> GetAllAccountToAccountTransactions()
        {
            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select * from trans_log where tran_name = 'To Bank Customer Transfer'", con);
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranFullReq = dr["tran_req"].ToString(),
                        TranFullResp = dr["tran_resp"].ToString(),
                        TranDate = dr["tran_resp_date"].ToString(),
                        TranStatus = dr["tran_status"].ToString(),
                        TranResult = dr["tran_resp_result"].ToString(),
                        UserId = dr["user_id"].ToString(),
                        AccountType = dr["tran_name"].ToString(),
                        TranReqAmount = dr["tran_amount"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public List<CustomerTransferReportViewModel> GetCreditAPITransaction()
        {
            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select * from trans_log where tran_name = 'AccountToCardTransfer'", con);
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranFullReq = dr["tran_req"].ToString(),
                        TranFullResp = dr["tran_resp"].ToString(),
                        TranDate = dr["tran_resp_date"].ToString(),
                        TranStatus = dr["tran_status"].ToString(),
                        TranResult = dr["tran_resp_result"].ToString(),
                        TranReqAmount = dr["tran_amount"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public List<CustomerTransferReportViewModel> GetCurrentCreditAPITransaction()
        {
            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select * from trans_log where tran_name = 'AccountToCardTransfer' and rownum <= 30 order by tran_resp_date desc", con);
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranFullReq = dr["tran_req"].ToString(),
                        TranFullResp = dr["tran_resp"].ToString(),
                        TranDate = dr["tran_resp_date"].ToString(),
                        TranStatus = dr["tran_status"].ToString(),
                        TranResult = dr["tran_resp_result"].ToString(),
                        TranReqAmount = dr["tran_amount"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public List<CustomerTransferReportViewModel> GetCreditAPITransaction(string branch, string status)
        {
            string sqlbranch = ""; string sqlstatus = "";

            if (branch != "000")
                sqlbranch = " and substr(users.account,3,3) = " + branch + " ";
            if (status != "All")
                sqlstatus = " and tran_status = '" + status + "' ";

            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select trans_log.tran_req,trans_log.tran_resp,trans_log.tran_resp_date,trans_log.tran_status,trans_log.tran_resp_result,trans_log.tran_amount from branchs,trans_log inner join users on users.user_id = trans_log.user_id where tran_name = 'AccountToCardTransfer'";
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                if (branch != "000")
                {
                    query += " and substr(users.account,3,3) = :branch";
                    cmd.Parameters.Add("branch", OracleType.VarChar).Value = branch;
                }
                if (status != "All")
                {
                    query += " and tran_status = :status";
                    cmd.Parameters.Add("status", OracleType.VarChar).Value = status;
                }
                cmd.CommandText = query;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranFullReq = dr["tran_req"].ToString(),
                        TranFullResp = dr["tran_resp"].ToString(),
                        TranDate = dr["tran_resp_date"].ToString(),
                        TranStatus = dr["tran_status"].ToString(),
                        TranResult = dr["tran_resp_result"].ToString(),
                        TranReqAmount = dr["tran_amount"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public List<CustomerTransferReportViewModel> GetCreditAPITransaction(string branch, string status, string fromdate, string todate)
        {
            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select distinct trans_log.tran_req,trans_log.tran_resp,trans_log.tran_resp_date,trans_log.tran_status,trans_log.tran_resp_result,trans_log.tran_amount from branchs,trans_log inner join users on users.user_id = trans_log.user_id where tran_name = 'AccountToCardTransfer' and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy')>= to_date(:fromdate,'mm/dd/yyyy') and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') <= to_date(:todate,'mm/dd/yyyy')";
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.Parameters.Add("fromdate", OracleType.VarChar).Value = fromdate;
                cmd.Parameters.Add("todate", OracleType.VarChar).Value = todate;
                if (branch != "000")
                {
                    query += " and substr(users.account,3,3) = :branch";
                    cmd.Parameters.Add("branch", OracleType.VarChar).Value = branch;
                }
                if (status != "All")
                {
                    query += " and tran_status = :status";
                    cmd.Parameters.Add("status", OracleType.VarChar).Value = status;
                }
                cmd.CommandText = query;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranFullReq = dr["tran_req"].ToString(),
                        TranFullResp = dr["tran_resp"].ToString(),
                        TranDate = dr["tran_resp_date"].ToString(),
                        TranStatus = dr["tran_status"].ToString(),
                        TranResult = dr["tran_resp_result"].ToString(),
                        TranReqAmount = dr["tran_amount"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }


        public List<CustomerTransferReportViewModel> GetUserReg(string branch, string status, string fromdate, string todate)
        {
            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select distinct trans_log.tran_req,trans_log.tran_resp,trans_log.tran_resp_date,trans_log.tran_status,trans_log.tran_resp_result,trans_log.tran_amount from branchs,trans_log inner join users on users.user_id = trans_log.user_id where tran_name = 'AccountToCardTransfer' and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy')>= to_date(:fromdate,'mm/dd/yyyy') and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') <= to_date(:todate,'mm/dd/yyyy')";
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.Parameters.Add("fromdate", OracleType.VarChar).Value = fromdate;
                cmd.Parameters.Add("todate", OracleType.VarChar).Value = todate;
                if (branch != "000")
                {
                    query += " and substr(users.account,3,3) = :branch";
                    cmd.Parameters.Add("branch", OracleType.VarChar).Value = branch;
                }
                if (status != "All")
                {
                    query += " and tran_status = :status";
                    cmd.Parameters.Add("status", OracleType.VarChar).Value = status;
                }
                cmd.CommandText = query;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranFullReq = dr["tran_req"].ToString(),
                        TranFullResp = dr["tran_resp"].ToString(),
                        TranDate = dr["tran_resp_date"].ToString(),
                        TranStatus = dr["tran_status"].ToString(),
                        TranResult = dr["tran_resp_result"].ToString(),
                        TranReqAmount = dr["tran_amount"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }



        public List<CustomerTransferReportViewModel> GetAccountTransferTransactions(string branch, string status)
        {
            string sqlbranch = ""; string sqlstatus = "";

            if (branch != "000")
                sqlbranch = " and substr(users.account,3,3) = " + branch + " ";
            if (status != "All")
                sqlstatus = " and tran_status = '" + status + "' ";

            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select trans_log.tran_req,trans_log.tran_resp,trans_log.tran_resp_date,trans_log.tran_status,trans_log.tran_resp_result,trans_log.tran_amount from trans_log inner join users on users.user_id = trans_log.user_id where tran_name = 'To Bank Customer Transfer'";
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                if (branch != "000")
                {
                    query += " and substr(users.account,3,3) = :branch";
                    cmd.Parameters.Add("branch", OracleType.VarChar).Value = branch;
                }
                if (status != "All")
                {
                    query += " and tran_status = :status";
                    cmd.Parameters.Add("status", OracleType.VarChar).Value = status;
                }
                cmd.CommandText = query;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranFullReq = dr["tran_req"].ToString(),
                        TranFullResp = dr["tran_resp"].ToString(),
                        TranDate = dr["tran_resp_date"].ToString(),
                        TranStatus = dr["tran_status"].ToString(),
                        TranResult = dr["tran_resp_result"].ToString(),
                        TranReqAmount = dr["tran_amount"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public List<CustomerTransferReportViewModel> FilteredAccountToAccountTransactions(string branch, string status, string fromdate, string todate , int pageNumber)
        {
            string sqlbranch = ""; string sqlstatus = ""; String sqlinc = "" ;

            if (branch != "000")
                sqlbranch = " and substr(users.account,3,3) = " + branch + " ";
            if (status != "All")
                sqlstatus = " and tran_status = '" + status + "' ";

            int offset = pageNumber * 500;
                sqlinc = " OFFSET " + offset + "  ROWS FETCH NEXT 500 ROWS ONLY ";

             
            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select distinct trans_log.tran_req,trans_log.tran_resp,trans_log.tran_resp_date,trans_log.tran_status,trans_log.tran_resp_result,trans_log.tran_amount from branchs,trans_log inner join users on users.user_id = trans_log.user_id where tran_name = 'To Bank Customer Transfer' and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') >= to_date(:fromdate,'mm/dd/yyyy') and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') <= to_date(:todate,'mm/dd/yyyy')";
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.Parameters.Add("fromdate", OracleType.VarChar).Value = fromdate;
                cmd.Parameters.Add("todate", OracleType.VarChar).Value = todate;
                if (branch != "000")
                {
                    query += " and substr(users.account,3,3) = :branch";
                    cmd.Parameters.Add("branch", OracleType.VarChar).Value = branch;
                }
                if (status != "All")
                {
                    query += " and tran_status = :status";
                    cmd.Parameters.Add("status", OracleType.VarChar).Value = status;
                }
                query += " " + sqlinc;
                cmd.CommandText = query;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                    
                        TranFullReq = dr["tran_req"].ToString(),
                        TranFullResp = dr["tran_resp"].ToString(),
                        TranDate = dr["tran_resp_date"].ToString(),
                        TranStatus = dr["tran_status"].ToString(),
                        TranResult = dr["tran_resp_result"].ToString(),
                        TranReqAmount = dr["tran_amount"].ToString()


                    }) ;
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }


        public List<CustomerTransferReportViewModel> FilteredAccountToAccountPrintTransactions(string branch, string status, string fromdate, string todate)
        {
            string sqlbranch = ""; string sqlstatus = ""; String sqlinc = "";

            if (branch != "000")
                sqlbranch = " and substr(users.account,3,3) = " + branch + " ";
            if (status != "All")
                sqlstatus = " and tran_status = '" + status + "' ";

            


            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select distinct trans_log.tran_req,trans_log.tran_resp,trans_log.tran_resp_date,trans_log.tran_status,trans_log.tran_resp_result,trans_log.tran_amount from branchs,trans_log inner join users on users.user_id = trans_log.user_id where tran_name = 'To Bank Customer Transfer' and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') >= to_date(:fromdate,'mm/dd/yyyy') and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') <= to_date(:todate,'mm/dd/yyyy')";
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.Parameters.Add("fromdate", OracleType.VarChar).Value = fromdate;
                cmd.Parameters.Add("todate", OracleType.VarChar).Value = todate;
                if (branch != "000")
                {
                    query += " and substr(users.account,3,3) = :branch";
                    cmd.Parameters.Add("branch", OracleType.VarChar).Value = branch;
                }
                if (status != "All")
                {
                    query += " and tran_status = :status";
                    cmd.Parameters.Add("status", OracleType.VarChar).Value = status;
                }
                cmd.CommandText = query;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                
               
                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranFullReq = dr["tran_req"].ToString(),
                        TranFullResp = dr["tran_resp"].ToString(),
                        TranDate = dr["tran_resp_date"].ToString(),
                        TranStatus = dr["tran_status"].ToString(),
                        TranResult = dr["tran_resp_result"].ToString(),
                        TranReqAmount = dr["tran_amount"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }


        public List<CustomerTransferReportViewModel> TotalTransactionsAmountsPerBranch(string branch_code, string fromdate, string todate)
        {
            string sqlbranch = "";
            if (branch_code != "000")
                sqlbranch = " where substr(users.account,3,3) = '" + branch_code + "'";

            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select tran_name as transaction_type,count(tran_name) as count,sum(tran_amount) as amount from trans_log inner join users on users.user_id = trans_log.user_id where to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') >= to_date(:fromdate,'mm/dd/yyyy') and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') <= to_date(:todate,'mm/dd/yyyy')";
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.Parameters.Add("fromdate", OracleType.VarChar).Value = fromdate;
                cmd.Parameters.Add("todate", OracleType.VarChar).Value = todate;
                if (branch_code != "000")
                {
                    query += " and substr(users.account,3,3) = :branch_code";
                    cmd.Parameters.Add("branch_code", OracleType.VarChar).Value = branch_code;
                }
                query += " group by tran_name";
                cmd.CommandText = query;

                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranResult = dr["transaction_type"].ToString(),
                        CurrencyCode = dr["count"].ToString(),
                        TranReqAmount = dr["amount"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }
 
        public List<CustomerTransferReportViewModel> TotalTransactionsAmountsPerBranch(string branch_code)
        {
            string sqlbranch = "";
            if (branch_code != "000")
                sqlbranch = " where substr(users.account,3,3) = '" + branch_code + "' ";

            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select tran_name as transaction_type,count(tran_name) as count,sum(tran_amount) as amount from trans_log inner join users on users.user_id = trans_log.user_id";
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                if (branch_code != "000")
                {
                    query += " where substr(users.account,3,3) = :branch_code";
                    cmd.Parameters.Add("branch_code", OracleType.VarChar).Value = branch_code;
                }
                query += " group by tran_name";
                cmd.CommandText = query;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranResult = dr["transaction_type"].ToString(),
                        CurrencyCode = dr["count"].ToString(),
                        TranReqAmount = dr["amount"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public List<UsersMangementViewModel> TotalCustomerLogbPerDate(string fromdate, string todate)
        {
            

            List<UsersMangementViewModel> transactions = new List<UsersMangementViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select user_log,last_login,decode(user_status,'A','Active','D','Deactive','DE','Deleted','U','Unauthorized','P','Pending','N/A') as status ,decode(catogry,'1','Personal','2','Operator','3','Authorizor','N/A') as category,last_log_ip,user_id from users where to_date(substr(CREATED_DATE,0,9),'dd-mon-yy') >= to_date(:fromdate,'mm/dd/yyyy') and to_date(substr(CREATED_DATE,0,9),'dd-mon-yy') <= to_date(:todate,'mm/dd/yyyy')", con);
                cmd.Parameters.Add("fromdate", OracleType.VarChar).Value = fromdate;
                cmd.Parameters.Add("todate", OracleType.VarChar).Value = todate;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new UsersMangementViewModel
                    {
                        Username = dr["user_log"].ToString(),
                        LoginTime = dr["last_login"].ToString(),
                        IpAddress = dr["last_log_ip"].ToString(),
                        UserStatus = dr["status"].ToString(),
                        Category = dr["category"].ToString(),
                        UserID = dr["user_id"].ToString()
                    });
                    
                }
                
                dr.Close();
                con.Close();
            }
            return transactions;
        }


        public List<CustomerTransferReportViewModel> GetTransactionPerBranch(string transaction_name, string fromdate, string todate)
        {
            string sqldate = "";// sqlbranch = "";
            //if (fromdate != "")
            //    sqldate = "  and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') >= to_date('" + fromdate + "','mm/dd/yyyy') and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') <= to_date('" + todate + "','mm/dd/yyyy') ";

            string sqltransactionname = "";
            if (transaction_name != "All")
                sqltransactionname = " and tran_name = '" + transaction_name + "' ";

            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select branch_name,count(tran_amount) as count,sum(tran_amount) as amount from branchs,trans_log inner join users on users.user_id = trans_log.user_id where substr(users.account,3,3) = branch_code and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') >= to_date(:fromdate,'mm/dd/yyyy') and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') <= to_date(:todate,'mm/dd/yyyy')";
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.Parameters.Add("fromdate", OracleType.VarChar).Value = fromdate;
                cmd.Parameters.Add("todate", OracleType.VarChar).Value = todate;
                if (transaction_name != "All")
                {
                    query += " and tran_name = :transaction_name";
                    cmd.Parameters.Add("transaction_name", OracleType.VarChar).Value = transaction_name;
                }
                query += " group by branch_name";
                cmd.CommandText = query;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranResult = dr["branch_name"].ToString(),
                        CurrencyCode = dr["count"].ToString(),
                        TranReqAmount = dr["amount"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public List<CustomerTransferReportViewModel> GetTransactionPerBranch(string transaction_name)
        {
            string sqltransactionname = "";
            if (transaction_name != "All")
                sqltransactionname = " and tran_name = '" + transaction_name + "' ";

            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select branch_name,count(tran_amount) as count,sum(tran_amount) as amount from branchs,trans_log inner join users on users.user_id = trans_log.user_id where substr(users.account,3,3) = branch_code";
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                if (transaction_name != "All")
                {
                    query += " and tran_name = :transaction_name";
                    cmd.Parameters.Add("transaction_name", OracleType.VarChar).Value = transaction_name;
                }
                query += " group by branch_name";
                cmd.CommandText = query;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranResult = dr["branch_name"].ToString(),
                        CurrencyCode = dr["count"].ToString(),
                        TranReqAmount = dr["amount"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public List<CustomerTransferReportViewModel> FilteredTransactionsInfo(string fromdate, string todate, string branch, string status, string accountnumber, string toaccount)
        {
            string sqldate = "", sqlbranch = "", sqlstatus = "", sqlaccountnumber = "", sqltoaccount = "";
            if (fromdate != "")
                sqldate = "  and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') >= to_date('" + fromdate + "','mm/dd/yyyy') and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') <= to_date('" + todate + "','mm/dd/yyyy') ";
            if (branch != "000")
                sqlbranch = " and substr(users.account,3,3) = '" + branch + "'  ";
            if (status != "All")
                sqlstatus = " and tran_status = '" + status + "' ";
            if (accountnumber != "")
                sqlaccountnumber = " and users.user_log = '" + accountnumber + "' ";
            if (toaccount != "")
                sqltoaccount = " and tran_req like '%" + toaccount + "%' ";

            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select * from trans_log inner join users on trans_log.user_id = users.user_id where users.user_id > 0";
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                if (fromdate != "")
                {
                    query += " and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') >= to_date(:fromdate,'mm/dd/yyyy') and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') <= to_date(:todate,'mm/dd/yyyy')";
                    cmd.Parameters.Add("fromdate", OracleType.VarChar).Value = fromdate;
                    cmd.Parameters.Add("todate", OracleType.VarChar).Value = todate;
                }
                if (branch != "000")
                {
                    query += " and substr(users.account,3,3) = :branch";
                    cmd.Parameters.Add("branch", OracleType.VarChar).Value = branch;
                }
                if (status != "All")
                {
                    query += " and tran_status = :status";
                    cmd.Parameters.Add("status", OracleType.VarChar).Value = status;
                }
                if (accountnumber != "")
                {
                    query += " and users.user_log = :accountnumber";
                    cmd.Parameters.Add("accountnumber", OracleType.VarChar).Value = accountnumber;
                }
                if (toaccount != "")
                {
                    query += " and tran_req like :toaccount";
                    cmd.Parameters.Add("toaccount", OracleType.VarChar).Value = "%" + toaccount + "%";
                }
                cmd.CommandText = query;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranFullReq = dr["tran_req"].ToString(),
                        TranFullResp = dr["tran_resp"].ToString(),
                        TranDate = dr["tran_req_date"].ToString(),
                        TranStatus = dr["tran_status"].ToString(),
                        ResponseStatus = dr["tran_resp_result"].ToString(),
                        TranName = dr["tran_name"].ToString(),
                        TranAmount = dr["tran_amount"].ToString(),
                        CustomerName = dr["user_name"].ToString(),
                        AccountNumber = dr["user_log"].ToString(),
                        UserMobile = dr["user_mobile"].ToString(),
                        TranID = dr["tran_id"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public List<CustomerTransferReportViewModel> DateFilteredGetCreditAPITransaction(string fromdate, string todate)
        {
            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select * from trans_log where tran_name = 'AccountToCardTransfer' and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') >= to_date(:fromdate,'mm/dd/yyyy') and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') <= to_date(:todate,'mm/dd/yyyy')", con);
                cmd.Parameters.Add("fromdate", OracleType.VarChar).Value = fromdate;
                cmd.Parameters.Add("todate", OracleType.VarChar).Value = todate;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranFullReq = dr["tran_req"].ToString(),
                        TranFullResp = dr["tran_resp"].ToString(),
                        TranDate = dr["tran_resp_date"].ToString(),
                        TranStatus = dr["tran_status"].ToString(),
                        TranResult = dr["tran_resp_result"].ToString(),
                        TranReqAmount = dr["tran_amount"].ToString()
                    });
                }

                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public List<CustomerTransferReportViewModel> DateFilteredGetviewcusTransaction(string fromdate, string todate)
        {
            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select * from trans_log where tran_name = 'AccountToCardTransfer' and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') >= to_date(:fromdate,'mm/dd/yyyy') and to_date(substr(TRAN_RESP_DATE,0,9),'dd-mon-yy') <= to_date(:todate,'mm/dd/yyyy')", con);
                cmd.Parameters.Add("fromdate", OracleType.VarChar).Value = fromdate;
                cmd.Parameters.Add("todate", OracleType.VarChar).Value = todate;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranFullReq = dr["tran_req"].ToString(),
                        TranFullResp = dr["tran_resp"].ToString(),
                        TranDate = dr["tran_resp_date"].ToString(),
                        TranStatus = dr["tran_status"].ToString(),
                        TranResult = dr["tran_resp_result"].ToString(),
                        TranReqAmount = dr["tran_amount"].ToString()
                    });
                }

                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public List<CustomerTransferReportViewModel> GetAlternativeAccountTransferReport()
        {
            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select * from trans_log where tran_name = 'AccountToCardTransfer'", con);
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranFullReq = dr["tran_req"].ToString(),
                        TranFullResp = dr["tran_resp"].ToString(),
                        TranDate = dr["tran_resp_date"].ToString(),
                        TranStatus = dr["tran_status"].ToString(),
                        TranResult = dr["tran_resp_result"].ToString(),
                        TranReqAmount = dr["tran_amount"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public List<CustomerTransferReportViewModel> GetAccountTransferReport()
        {
            List<CustomerTransferReportViewModel> transactions = new List<CustomerTransferReportViewModel>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select * from trans_log where tran_name <> 'AccountToCardTransfer'", con);
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    transactions.Add(new CustomerTransferReportViewModel
                    {
                        TranFullReq = dr["tran_req"].ToString(),
                        TranFullResp = dr["tran_resp"].ToString(),
                        TranDate = dr["tran_resp_date"].ToString(),
                        TranStatus = dr["tran_status"].ToString(),
                        TranResult = dr["tran_resp_result"].ToString(),
                        TranReqAmount = dr["tran_amount"].ToString()
                    });
                }
                dr.Close();
                con.Close();
            }
            return transactions;
        }

        public string GetCurrencyName(string CurrencyCode)
        {
            string branchs = "", CurrencyName = "";

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select curr_name from currency where curr_code = :CurrencyCode", con);
                cmd.Parameters.Add("CurrencyCode", OracleType.VarChar).Value = CurrencyCode;

                con.Open();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        if (dataReader["curr_name"] != DBNull.Value)
                        {
                            CurrencyName = (string)dataReader["curr_name"];
                        }
                    }
                    return CurrencyName;
                }
            }
        }
        public Boolean AuthorizeAccountType(string account_type_code)
        {
            Boolean response = false;
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("update act_types set act_sts = '1' where act_type_code = :account_type_code", con);
                cmd.Parameters.Add("account_type_code", OracleType.VarChar).Value = account_type_code;
                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                if (result == -1)
                {
                    response = false;
                }
                else
                {
                    response = true;
                }
            }
            return response;
        }

        public Boolean AuthorizeBranch(string branch_code)
        {
            Boolean response = false;
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("update branchs set branch_sts = '1' where branch_code = :branch_code", con);
                cmd.Parameters.Add("branch_code", OracleType.VarChar).Value = branch_code;
                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                if (result == -1)
                {
                    response = false;
                }
                else
                {
                    response = true;
                }
            }
            return response;
        }

        public Boolean RejectAccountType(string account_type_code)
        {
            Boolean response = false;
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("delete from act_types where act_type_code = :account_type_code", con);
                cmd.Parameters.Add("account_type_code", OracleType.VarChar).Value = account_type_code;
                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                if (result == -1)
                {
                    response = false;
                }
                else
                {
                    response = true;
                }
            }
            return response;
        }

        public Boolean RejectBranch(string branch_code)
        {
            Boolean response = false;
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("delete from branchs where branch_code = :branch_code", con);
                cmd.Parameters.Add("branch_code", OracleType.VarChar).Value = branch_code;
                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                if (result == -1)
                {
                    response = false;
                }
                else
                {
                    response = true;
                }
            }
            return response;
        }



        public List<AccountTypeModel> GetAllAccountTypes()
        {
            List<AccountTypeModel> accountTypes = new List<AccountTypeModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select act_type_code,act_name,act_name_ar,act_type_id from act_types", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        accountTypes.Add(new AccountTypeModel
                        {
                            account_type_code = dr[0].ToString(),
                            account_type = dr[1].ToString(),
                            account_type_arabic = dr[2].ToString(),
                            account_type_no = dr[3].ToString(),
                        });
                    }
                }
            }
            return accountTypes;
        }

        public List<AccountTypeModel> GetAllPendingAccountTypes()
        {
            List<AccountTypeModel> accountTypes = new List<AccountTypeModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                //OracleCommand cmd = new OracleCommand("select act_type_code,act_name,act_name_ar,act_type_id from act_types where act_sts = 'P'", con);
                OracleCommand cmd = new OracleCommand("select act_type_code,act_name,act_name_ar,act_type_id from act_types where act_sts = '1'", con);

                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        accountTypes.Add(new AccountTypeModel
                        {
                            account_type_code = dr[0].ToString(),
                            account_type = dr[1].ToString(),
                            account_type_arabic = dr[2].ToString(),
                            account_type_no = dr[3].ToString(),
                        });
                    }
                }
            }
            return accountTypes;
        }

        public List<BranchModel> GetAllBranchs()
        {
            List<BranchModel> branchs = new List<BranchModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select BRANCH_CODE,BRANCH_NAME,decode(BRANCH_STS,'0','Deactive','1','Active'),BRANCH_CODE_NO,BRANCH_NAME_AR from branchs", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        branchs.Add(new BranchModel
                        {
                            branch_code = dr[0].ToString(),
                            branch_name = dr[1].ToString(),
                            branch_status = dr[2].ToString(),
                            branch_code_no = dr[3].ToString(),
                            branch_name_arabic = dr[4].ToString(),
                        });
                    }
                }
            }
            return branchs;
        }

        public List<Charter> getUsersBranchsCount()
        {
            List<Charter> users = new List<Charter>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select branch_name,count(user_id) as count from users inner join branchs on substr(account,3,3) = branch_code group by branch_name", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        users.Add(new Charter
                        {
                            name = dr[0].ToString(),
                            value = dr[1].ToString()
                        });
                    }
                }
            }
            return users;
        }

        public List<Charter> getBranchsTransactionsCount()
        {
            List<Charter> users = new List<Charter>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select branchs.branch_name,count(tm.tran_id) as count from ( select * from users inner join trans_log on users.user_id = trans_log.user_id ) tm inner join branchs on substr(account,3,3) = branch_code group by branchs.branch_name", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        users.Add(new Charter
                        {
                            name = dr[0].ToString(),
                            value = dr[1].ToString()
                        });
                    }
                }
            }
            return users;
        }

        public List<Charter> getAllStatuses()
        {
            List<Charter> users = new List<Charter>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select decode(user_status,'U','Authorized','P','Pending','D','Deactivated','A','Active','B','Blocked','DE','Deleted') as status,count(user_id) as count from users group by user_status", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        users.Add(new Charter
                        {
                            name = dr[0].ToString(),
                            value = dr[1].ToString()
                        });
                    }
                }
            }
            return users;
        }

        public List<Transferlimit> GetServicesByRole(string roleid)
        {
            List<Transferlimit> branchs = new List<Transferlimit>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select transfer_limit.tran_id,services.service_name,transfer_limit.amount_limit_transaction,transfer_limit.amount_limit_day,transfer_limit.tranno_limit_day from transfer_limit inner join services on transfer_limit.service_id = services.service_id and role_id = :roleid and service_status = '1'", con);
                cmd.Parameters.Add("roleid", OracleType.VarChar).Value = roleid;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        branchs.Add(new Transferlimit
                        {
                            tran_id = dr[0].ToString(),
                            servicename = dr[1].ToString(),
                            amount_limit = dr[2].ToString(),
                            daily_limit = dr[3].ToString(),
                            number_limit = dr[4].ToString(),
                        });
                    }
                }
            }
            return branchs;
        }

        public Transferlimit GetSingleServicesByRole(string roleid)
        {
            Transferlimit service = new Transferlimit();
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select transfer_limit.tran_id,services.service_name,transfer_limit.amount_limit_transaction,transfer_limit.amount_limit_day,transfer_limit.tranno_limit_day from transfer_limit inner join services on transfer_limit.service_id = services.service_id and role_id = :roleid and service_status = '1'", con);
                cmd.Parameters.Add("roleid", OracleType.VarChar).Value = roleid;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        service.tran_id = dr[0].ToString();
                        service.servicename = dr[1].ToString();
                        service.amount_limit = dr[2].ToString();
                        service.daily_limit = dr[3].ToString();
                        service.number_limit = dr[4].ToString();
                    }
                }
            }
            return service;
        }

        public Boolean updatelimit(Transferlimit model)
        {
            Boolean response = false;
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("update transfer_limit set amount_limit_transaction = :amount_limit, amount_limit_day = :daily_limit, tranno_limit_day = :number_limit where tran_id = :tran_id", con);
                cmd.Parameters.Add("amount_limit", OracleType.VarChar).Value = model.amount_limit;
                cmd.Parameters.Add("daily_limit", OracleType.VarChar).Value = model.daily_limit;
                cmd.Parameters.Add("number_limit", OracleType.VarChar).Value = model.number_limit;
                cmd.Parameters.Add("tran_id", OracleType.VarChar).Value = model.tran_id;
                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                if (result == -1)
                {
                    response = false;
                }
                else
                {
                    response = true;
                }
            }
            return response;
        }

        public List<BranchModel> GetAllPendingBranchs()
        {
            List<BranchModel> branchs = new List<BranchModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select BRANCH_CODE,BRANCH_NAME,decode(BRANCH_STS,'0','Deactive','1','Active','P','Pending'),BRANCH_CODE_NO,BRANCH_NAME_AR from branchs where branch_sts = 'P'", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        branchs.Add(new BranchModel
                        {
                            branch_code = dr[0].ToString(),
                            branch_name = dr[1].ToString(),
                            branch_status = dr[2].ToString(),
                            branch_code_no = dr[3].ToString(),
                            branch_name_arabic = dr[4].ToString(),
                        });
                    }
                }
            }
            return branchs;
        }

        public List<CurrencyModel> GetAllCurrencies()
        {
            List<CurrencyModel> currencies = new List<CurrencyModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select curr_code,curr_name,curr_sumry, DECODE (curr_sts,'1','Active','DeActive') from currency", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        currencies.Add(new CurrencyModel
                        {
                            currency_code = dr[0].ToString(),
                            currency_name = dr[1].ToString(),
                            currency_summary = dr[2].ToString(),
                            currency_status = dr[3].ToString(),
                        });
                    }
                }
            }
            return currencies;
        }

        public List<LatestTransactions> getAllTransactions()
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "getdetailedtransactions";

                OracleParameter pr1 = new OracleParameter("status", OracleType.VarChar, 2000);
                pr1.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pr1);
                OracleParameter pr2 = new OracleParameter("ti", OracleType.VarChar, 2000);
                pr2.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pr2);
                OracleParameter pr3 = new OracleParameter("tn", OracleType.VarChar, 2000);
                pr3.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pr3);
                OracleParameter pr4 = new OracleParameter("tst", OracleType.VarChar, 2000);
                pr4.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pr4);
                OracleParameter pr5 = new OracleParameter("trr", OracleType.VarChar, 2000);
                pr5.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pr5);
                OracleParameter pr6 = new OracleParameter("tran_req", OracleType.VarChar, 2000);
                pr6.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pr6);
                OracleParameter pr7 = new OracleParameter("tran_date", OracleType.VarChar, 2000);
                pr7.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pr7);
                OracleParameter pr8 = new OracleParameter("tran_amount", OracleType.VarChar, 2000);
                pr8.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pr8);
                cmd.Parameters.Add("p_cr", OracleType.Cursor).Direction = ParameterDirection.Output;

                con.Open();
                List<LatestTransactions> list = new List<LatestTransactions>();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        LatestTransactions obj = new LatestTransactions();

                        if (dataReader["tran_id"] != DBNull.Value)
                        {
                            //obj.AccountID = (int)dataReader["acc_id"];
                            obj.TranId = Convert.ToInt32(dataReader["tran_id"]);
                        }
                        if (dataReader["tran_name"] != DBNull.Value)
                        {
                            obj.TranName = (string)dataReader["tran_name"];
                        }
                        if (dataReader["tran_status"] != DBNull.Value)
                        {
                            obj.TranStatus = (string)dataReader["tran_status"];
                        }
                        if (dataReader["tran_resp_result"] != DBNull.Value)
                        {
                            obj.TranResult = (string)dataReader["tran_resp_result"];
                        }
                        if (dataReader["tran_req"] != DBNull.Value)
                        {
                            obj.tranreq = (string)dataReader["tran_req"];
                        }
                        if (dataReader["tran_date"] != DBNull.Value)
                        {
                            obj.trandate = (string)dataReader["tran_date"];
                        }
                        if (dataReader["tran_amount"] != DBNull.Value)
                        {
                            obj.tranamount = (string)dataReader["tran_amount"];
                        }
                        list.Add(obj);
                    }
                    return list;
                }
            }
        }

        public string getgroupmaxid()
        {
            string maxcount = "";
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("GETGROUPMAXID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                OracleParameter p3 = new OracleParameter("status", OracleType.VarChar, 2000);
                p3.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p3);
                OracleParameter p1 = new OracleParameter("maxid", OracleType.VarChar, 2000);
                p1.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add("p_cr", OracleType.Cursor).Direction = ParameterDirection.Output;

                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    maxcount = dr["MAX(ID)"].ToString();
                }

                return maxcount;
            }
        }

        public int insertservice(ServiceInsertModel model)
        {

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("INSERT INTO SERVICES (service_id,service_name,service_code,service_status) VALUES (SERVSEQ.nextval,:service_name, RPAD(SERVSEQ.currval, 5, '0'),'A')", con);
                cmd.Parameters.Add("service_name", OracleType.VarChar).Value = model.service_name;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        public BranchModel getbranch(string branchcode)
        {
            BranchModel branch = new BranchModel();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select branch_code,branch_name,branch_sts,branch_code_no,branch_db_link,branch_name_ar from branchs where branch_code = :branchcode", con);
                cmd.Parameters.Add("branchcode", OracleType.VarChar).Value = branchcode;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    branch.branch_code = dr["branch_code"].ToString();
                    branch.branch_name = dr["branch_name"].ToString();
                    branch.branch_status = dr["branch_sts"].ToString();
                    branch.branch_name_arabic = dr["branch_name_ar"].ToString();
                }
                else
                {
                    dr.Close();
                }
                dr.Close();
                con.Close();
            }
            return branch;
        }

        public int deletebranch(string branch_code)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("delete from branchs where branch_code = :branch_code", con);
                cmd.Parameters.Add("branch_code", OracleType.VarChar).Value = branch_code;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        public int Insertbranch(string branch_code, string branch_name, string branch_name_arabic, string branch_status, string branch_code_no)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "insertbranch";

                cmd.Parameters.Add("BRANCHCODE", OracleType.VarChar).Value = branch_code;
                cmd.Parameters.Add("BRANCHNAME", OracleType.VarChar).Value = branch_name;
                cmd.Parameters.Add("BRANCHSTATUS", OracleType.VarChar).Value = branch_status;
                cmd.Parameters.Add("BRANCHCODENO", OracleType.VarChar).Value = branch_code_no;
                cmd.Parameters.Add("BRANCHNAMEARABIC", OracleType.VarChar).Value = branch_name_arabic;
                OracleParameter p3 = new OracleParameter("res", OracleType.Int32);
                p3.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p3);
                OracleParameter p4 = new OracleParameter("errcode", OracleType.VarChar, 2000);
                p4.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p4);
                OracleParameter p5 = new OracleParameter("errmsg", OracleType.VarChar, 2000);
                p5.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p5);

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        public List<SelectListItem> GetDisputeReasons()
        {
            using (OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                List<SelectListItem> dispute_reasons = new List<SelectListItem>();
                OracleCommand cmd = new OracleCommand("GETDISPUTEREASONS", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("cur", OracleType.Cursor).Direction = System.Data.ParameterDirection.Output;
                con.Open();
                IDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    SelectListItem reason = new SelectListItem();
                    reason.Text = sdr["REASON"].ToString();
                    reason.Value = sdr["ID"].ToString();
                    dispute_reasons.Add(reason);
                }
                sdr.Close();
                con.Close();
                return dispute_reasons;
            }
        }

        public List<Dispute_Action_Model> GetDisputeActions()
        {
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                List<Dispute_Action_Model> actions = new List<Dispute_Action_Model>();
                OracleCommand command = new OracleCommand("GETDISPUTEACTIONS", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("cur", OracleType.Cursor).Direction = System.Data.ParameterDirection.Output;

                connection.Open();
                IDataReader idr = command.ExecuteReader();
                while (idr.Read())
                {
                    Dispute_Action_Model action = new Dispute_Action_Model();
                    action.id = idr["ID"].ToString();
                    action.action = idr["ACTION"].ToString();
                    action.action_arabic = idr["ACTION_AR"].ToString();
                    action.status = idr["STATUS"].ToString();
                    action.action_code = idr["ACTION_CODE"].ToString();
                    action.action_status = idr["ACTION_STATUS"].ToString();
                    actions.Add(action);
                }
                idr.Close();
                connection.Close();
                return actions;
            }
        }

        public List<Dispute> GetAllDisputes()
        {
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                List<Dispute> Disputes = new List<Dispute>();
                OracleCommand command = new OracleCommand("GETALLDISPUTES", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("cur", OracleType.Cursor).Direction = System.Data.ParameterDirection.Output;

                connection.Open();
                IDataReader idr = command.ExecuteReader();
                while (idr.Read())
                {
                    Dispute dispute = new Dispute();
                    dispute.id = idr["ID"].ToString();
                    dispute.ACCOUNTFROM = idr["ACCOUNTFROM"].ToString();
                    //dispute.ACCOUNTFROM = getbranchnameenglish(dispute.ACCOUNTFROM.Substring(2, 3) + " - " + getaccounttype(dispute.ACCOUNTFROM.Substring(5, 5)) + " - " + dispute.ACCOUNTFROM.Substring(13));
                    dispute.ACCOUNTTO = idr["ACCOUNTTO"].ToString();
                    dispute.AMOUNT = idr["AMOUNT"].ToString();
                    dispute.DATETIME = idr["DATETIME"].ToString();
                    dispute.STATUS = idr["STATUS"].ToString();
                    dispute.USER_LOG = idr["USER_LOG"].ToString();
                    dispute.REASON = idr["REASON"].ToString();
                    dispute.USER_ENTRY = idr["USER_ENTRY"].ToString();
                    dispute.AUTHORIZOR = idr["AUTHORIZOR"].ToString();
                    dispute.FT = idr["FT"].ToString();
                    dispute.RRN = idr["RRN"].ToString();
                    dispute.NARRIATION = idr["NARRIATION"].ToString();
                    Disputes.Add(dispute);
                }
                idr.Close();
                connection.Close();
                return Disputes;
            }
        }

        public List<Comment> GetDisputeComments(string dispute_id)
        {
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                List<Comment> Comments = new List<Comment>();
                OracleCommand command = new OracleCommand("getdisputecomments", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("dp_id", OracleType.VarChar).Value = dispute_id;
                command.Parameters.Add("cur", OracleType.Cursor).Direction = System.Data.ParameterDirection.Output;

                connection.Open();
                IDataReader idr = command.ExecuteReader();
                while (idr.Read())
                {
                    Comment comment = new Comment();
                    comment.ID = idr["ID"].ToString();
                    comment.Dispute_id = idr["DISPUTE_ID"].ToString();
                    comment.Status = idr["STATUS"].ToString();
                    comment.Comment_text = idr["COMMENTS"].ToString();
                    comment.User_entry = idr["USER_ENTRY"].ToString();
                    comment.Reason = idr["REASON"].ToString();
                    comment.Action = idr["ACTION"].ToString();
                    Comments.Add(comment);
                }
                idr.Close();
                connection.Close();
                return Comments;
            }
        }

        public int InsertDispute(CustomerTransferReportViewModel model, string admin)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "INSERTDISPUTE";

                cmd.Parameters.Add("ACCOUNTFROM", OracleType.VarChar).Value = model.TranFromAccount.ToString();
                cmd.Parameters.Add("ACCOUNTTO", OracleType.VarChar).Value = model.TranToAccount.ToString();
                cmd.Parameters.Add("AMOUNT", OracleType.VarChar).Value = model.TranAmount.ToString();
                cmd.Parameters.Add("DATETIME", OracleType.VarChar).Value = model.TranDate.ToString();
                cmd.Parameters.Add("STATUS", OracleType.VarChar).Value = model.TranStatus.ToString();
                cmd.Parameters.Add("USER_LOG", OracleType.VarChar).Value = model.User_log.ToString();
                cmd.Parameters.Add("REASON_CODE", OracleType.VarChar).Value = model.selected_dispute.ToString();
                cmd.Parameters.Add("USER_ENTRY", OracleType.VarChar).Value = admin.ToString();
                cmd.Parameters.Add("AUTHORIZOR", OracleType.VarChar).Value = "N/A";
                cmd.Parameters.Add("FT", OracleType.VarChar).Value = model.FT.ToString();
                cmd.Parameters.Add("RRN", OracleType.VarChar).Value = model.RRN.ToString();
                cmd.Parameters.Add("NARRIATION", OracleType.VarChar).Value = model.Narriation.ToString();
                cmd.Parameters.Add("COMMENTS", OracleType.VarChar).Value = model.Comment.ToString();
                cmd.Parameters.Add("TRANSACTIONID", OracleType.VarChar).Value = model.TranID.ToString();
                OracleParameter p = new OracleParameter("STATUSOUT", OracleType.VarChar, 2000);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        //public Dispute GetDispute(string dispute_id)
        //{
        //    using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        //    {
        //        Dispute dispute = new Dispute();
        //        OracleCommand command = new OracleCommand("GETDISPUTE", connection);
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.Parameters.Add("dp_id", OracleType.VarChar).Value = dispute_id;
        //        command.Parameters.Add("cur", OracleType.Cursor).Direction = System.Data.ParameterDirection.Output;

        //        connection.Open();
        //        IDataReader idr = command.ExecuteReader();
        //        while (idr.Read())
        //        {
        //            dispute.id = idr["ID"].ToString();
        //            dispute.ACCOUNTFROM = idr["ACCOUNTFROM"].ToString();
        //            //dispute.ACCOUNTFROM = getbranchnameenglish(dispute.ACCOUNTFROM.Substring(2, 3) + " - " + getaccounttype(dispute.ACCOUNTFROM.Substring(5, 5)) + " - " + dispute.ACCOUNTFROM.Substring(13));
        //            dispute.ACCOUNTTO = idr["ACCOUNTTO"].ToString();
        //            dispute.AMOUNT = idr["AMOUNT"].ToString();
        //            dispute.DATETIME = idr["DATETIME"].ToString();
        //            dispute.STATUS = idr["STATUS"].ToString();
        //            dispute.USER_LOG = idr["USER_LOG"].ToString();
        //            dispute.REASON = idr["REASON"].ToString();
        //            dispute.USER_ENTRY = idr["USER_ENTRY"].ToString();
        //            dispute.AUTHORIZOR = idr["AUTHORIZOR"].ToString();
        //            dispute.FT = idr["FT"].ToString();
        //            dispute.RRN = idr["RRN"].ToString();
        //            dispute.NARRIATION = idr["NARRIATION"].ToString();
        //            dispute.TRANSACTIONID = idr["TRANSACTIONID"].ToString();
        //        }
        //        idr.Close();
        //        connection.Close();
        //        return dispute;
        //    }
        //}

        public Dispute GetDispute(string dispute_id)
        {
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                Dispute dispute = new Dispute();
               
                OracleCommand command = new OracleCommand("GETDISPUTE", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("dp_id", OracleType.VarChar).Value = dispute_id;
                command.Parameters.Add("cur", OracleType.Cursor).Direction = System.Data.ParameterDirection.Output;

                connection.Open();
                IDataReader idr = command.ExecuteReader();
                while (idr.Read())
                {
                  
                    dispute.id = idr["ID"].ToString();
                    dispute.ACCOUNTFROM = idr["ACCOUNTFROM"].ToString();
                    //dispute.ACCOUNTFROM = getbranchnameenglish(dispute.ACCOUNTFROM.Substring(2, 3) + " - " + getaccounttype(dispute.ACCOUNTFROM.Substring(5, 5)) + " - " + dispute.ACCOUNTFROM.Substring(13));
                    dispute.ACCOUNTTO = idr["ACCOUNTTO"].ToString();
                    dispute.AMOUNT = idr["AMOUNT"].ToString();
                    dispute.DATETIME = idr["DATETIME"].ToString();
                    dispute.STATUS = idr["STATUS"].ToString();
                    dispute.USER_LOG = idr["USER_LOG"].ToString();
                    dispute.REASON = idr["REASON"].ToString();
                    dispute.USER_ENTRY = idr["USER_ENTRY"].ToString();
                    dispute.AUTHORIZOR = idr["AUTHORIZOR"].ToString();
                    dispute.FT = idr["FT"].ToString();
                    dispute.RRN = idr["RRN"].ToString();
                    dispute.NARRIATION = idr["NARRIATION"].ToString();
                    dispute.TRANSACTIONID = idr["TRANSACTIONID"].ToString();
                }
                idr.Close();
                connection.Close();
                return dispute;
            }
        }

        public int InsertComment(CustomerTransferReportViewModel model, string admin)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "INSERTCOMMENT";

                cmd.Parameters.Add("DISPUTE_ID", OracleType.VarChar).Value = model.dispute_id;
                cmd.Parameters.Add("STATUS", OracleType.VarChar).Value = "U";
                cmd.Parameters.Add("COMMENTS", OracleType.VarChar).Value = model.Comment.ToString();
                cmd.Parameters.Add("USER_ENTRY", OracleType.VarChar).Value = admin;
                cmd.Parameters.Add("REASON_CODE", OracleType.VarChar).Value = model.REASON_CODE.ToString();
                cmd.Parameters.Add("ACTION_CODE", OracleType.VarChar).Value = model.ACTION_CODE.ToString();
                OracleParameter p = new OracleParameter("STATUSOUT", OracleType.VarChar, 2000);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                return result;
            }
        }


        public int Updatebranch(BranchModel model)
        {

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update branchs set branch_name = :branch_name, branch_sts = :branch_status, branch_code_no = :branch_code_no, branch_name_ar = :branch_name_ar where branch_code = :branch_code", con);
                cmd.Parameters.Add("branch_name", OracleType.VarChar).Value = model.branch_name;
                cmd.Parameters.Add("branch_status", OracleType.VarChar).Value = model.branch_status;
                cmd.Parameters.Add("branch_code_no", OracleType.VarChar).Value = model.branch_code_no;
                cmd.Parameters.Add("branch_name_ar", OracleType.VarChar).Value = model.branch_name_arabic;
                cmd.Parameters.Add("branch_code", OracleType.VarChar).Value = model.branch_code;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }
        public string MD5Hash(string text)
        {
            MD5 md5H = MD5.Create();
            //convert the input string to a byte array and compute its hash
            byte[] data = md5H.ComputeHash(Encoding.UTF8.GetBytes(text));
            // create a new stringbuilder to collect the bytes and create a string
            StringBuilder sB = new StringBuilder();
            //loop through each byte of hashed data and format each one as a hexadecimal string
            for (int i = 0; i < data.Length; i++)
            {
                sB.Append(data[i].ToString("x2"));
            }
            //return hexadecimal string
            return sB.ToString();
        }
        public int insert(userInsertModel model)
        {
            OracleCommand cmd;

            using (OracleConnection con = new OracleConnection(conString))
            {
                if (model.roleid == "2")
                {
                    cmd = new OracleCommand("INSERT INTO jsb_security_master (USER_LOG,USER_PWD,USER_NAME,USER_LAST_LOGIN,USER_ID,ROLEID,USER_BRANCHUSER_STATUS,ROLEIDCREATED,USER_FIRST_LOGIN_STATUS,USER_EMAIL,USER_MOBILE) VALUES(:user_log,:user_pwd,:user_name,'T',CP_USERID.nextval,:roleid,'000','UA',:roleidcreated,'T',:user_email,:user_mobile)", con);
                }
                else
                {
                    cmd = new OracleCommand("INSERT INTO jsb_security_master (USER_LOG,USER_PWD,USER_NAME,USER_LAST_LOGIN,USER_ID,ROLEID,USER_BRANCH,USER_STATUS,ROLEIDCREATED,USER_FIRST_LOGIN_STATUS,USER_EMAIL,USER_MOBILE) VALUES(:user_log,:user_pwd,:user_name,'T',CP_USERID.nextval,:roleid,:branch_code,'UA',:roleidcreated,'T',:user_email,:user_mobile)", con);
                    cmd.Parameters.Add("branch_code", OracleType.VarChar).Value = model.BranchCode;
                }

                cmd.Parameters.Add("user_log", OracleType.VarChar).Value = model.user_name;
                cmd.Parameters.Add("user_pwd", OracleType.VarChar).Value = model.Password;
                cmd.Parameters.Add("user_name", OracleType.VarChar).Value = model.name;
                cmd.Parameters.Add("roleid", OracleType.VarChar).Value = model.roleid;
                cmd.Parameters.Add("roleidcreated", OracleType.VarChar).Value = model.roleidcreated;
                cmd.Parameters.Add("user_email", OracleType.VarChar).Value = model.email;
                cmd.Parameters.Add("user_mobile", OracleType.VarChar).Value = model.phone;

                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }
        public int Update(userUpdateModel model)
        {

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update jsb_security_master set user_log = :user_log, user_name = :user_name, roleid = :roleid, user_branch = :user_branch, user_mobile = :user_mobile, user_email = :user_email, user_status = 'RED' where user_id = :user_id", con);
                cmd.Parameters.Add("user_log", OracleType.VarChar).Value = model.user_name;
                cmd.Parameters.Add("user_name", OracleType.VarChar).Value = model.name;
                cmd.Parameters.Add("roleid", OracleType.VarChar).Value = model.roleid;
                cmd.Parameters.Add("user_branch", OracleType.VarChar).Value = model.BranchCode;
                cmd.Parameters.Add("user_mobile", OracleType.VarChar).Value = model.phone;
                cmd.Parameters.Add("user_email", OracleType.VarChar).Value = model.email;
                cmd.Parameters.Add("user_id", OracleType.Int32).Value = model.user_id;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }
        public int UpdateService(ServiceUpdateModel model)
        {

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update SERVICES set service_name = :service_name, service_status = :service_status where service_id = :service_id", con);
                cmd.Parameters.Add("service_name", OracleType.VarChar).Value = model.service_name;
                cmd.Parameters.Add("service_status", OracleType.VarChar).Value = model.service_status;
                cmd.Parameters.Add("service_id", OracleType.Int32).Value = int.Parse(model.service_id);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }
        public userUpdateModel getuserdata(int id)
        {
            userUpdateModel updatemodel = new userUpdateModel();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select user_id, user_log, user_name, roleid, user_branch, user_mobile, user_email from jsb_security_master where user_id = :id", con);
                cmd.Parameters.Add("id", OracleType.Int32).Value = id;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {

                    updatemodel.user_id = Convert.ToInt32(dr["user_id"].ToString());
                    updatemodel.roleid = dr["roleid"].ToString();
                    updatemodel.BranchCode = dr["user_branch"].ToString();
                    updatemodel.user_name = dr["user_log"].ToString();
                    updatemodel.name = dr["user_name"].ToString();
                    updatemodel.phone = dr["user_mobile"].ToString();
                    updatemodel.email = dr["user_email"].ToString();
                }
                else
                {
                    dr.Close();
                }
                dr.Close();
                con.Close();
            }
            return updatemodel;
        }
        public ServiceUpdateModel getServiccedata(int id)
        {
            ServiceUpdateModel updatemodel = new ServiceUpdateModel();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select service_id,service_name,service_code,service_status from SERVICES where service_id = :id", con);
                cmd.Parameters.Add("id", OracleType.Int32).Value = id;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {

                    updatemodel.service_id = dr["service_id"].ToString();
                    updatemodel.service_code = dr["service_code"].ToString();
                    updatemodel.service_name = dr["service_name"].ToString();
                    updatemodel.service_status = dr["service_status"].ToString();

                }
                else
                {
                    dr.Close();
                }
                dr.Close();
                con.Close();
            }
            return updatemodel;
        }
        public List<SelectListItem> Populatecpanelstatuses()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //using (OracleConnection con = new OracleConnection(conString))
            //{
            //    string query = "select service_id,service_status from services";

            //    using (OracleCommand cmd = new OracleCommand(query))
            //    {
            //        cmd.Connection = con;
            //        con.Open();
            //        using (OracleDataReader sdr = cmd.ExecuteReader())
            //        {
            //            while (sdr.Read())
            //            {
            //                items.Add(new SelectListItem
            //                {
            //                    Text = sdr["service_status"].ToString(),
            //                    Value = sdr["service_id"].ToString(),
            //                });
            //            }
            //        }
            //        con.Close();
            //    }
            //}
            items.Add(new SelectListItem
            {
                Text = "A",
                Value = "1",
            });
            items.Add(new SelectListItem
            {
                Text = "DE",
                Value = "2",
            });
            return items;
        }

        public List<UsersChartsModel> GetChartsData(string branchcode)
        {
            List<UsersChartsModel> list = new List<UsersChartsModel>();
            List<float> numberslist = new List<float>();
            float sum = 0; int count = 0;
            using (OracleConnection con = new OracleConnection(conString))
            {
                //string query = "select cat_name,count(users.user_id) as users from users inner join category on users.catogry = category.cat_id inner join security_master on SUBSTR(users.account,3,3) = '" + branchcode + "' group by cat_name";
                //temp query
                string query = "select cat_name,count(users.user_id) as users from users inner join category on users.catogry = category.cat_id group by cat_name";

                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            list.Add(new UsersChartsModel
                            {
                                category = sdr["cat_name"].ToString(),
                                userscount = int.Parse(sdr["users"].ToString()),
                            });
                            numberslist.Add(int.Parse(sdr["users"].ToString()));
                        }
                    }
                    con.Close();
                }
                foreach (var item in numberslist)
                {
                    sum = sum + item;
                }

                for (int i = 0; i < numberslist.Count; i++)
                {
                    numberslist[i] = numberslist[i] * 100 / sum;
                }

                foreach (var item in list)
                {
                    item.userscount = numberslist[count];
                    count++;
                }
            }
            return list;
        }

        public List<TransactionsDetailsModel> GetTransactionsDetails(string branchcode)
        {
            List<TransactionsDetailsModel> result = new List<TransactionsDetailsModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                //string query = "select distinct trans_log.tran_name as transaction_name,count(trans_log.tran_name) as transactions_count from trans_log inner join users on trans_log.user_id = users.user_id where SUBSTR(users.def_acc,3,3) = '" + branchcode + "' group by tran_name";
                //temp query 
                string query = "select distinct trans_log.tran_name as transaction_name,count(trans_log.tran_name) as transactions_count from trans_log inner join users on trans_log.user_id = users.user_id group by tran_name";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            result.Add(new TransactionsDetailsModel
                            {
                                transactiontype = sdr["Transaction_name"].ToString(),
                                transactioncount = int.Parse(sdr["Transactions_count"].ToString())
                            });
                        }
                    }
                    con.Close();
                }
            }
            return result;
        }

        public List<TransactionsDetailsModel> GetTransactionsCountsAndAmounts()
        {
            List<TransactionsDetailsModel> result = new List<TransactionsDetailsModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                //string query = "select distinct trans_log.tran_name as transaction_name,count(trans_log.tran_name) as transactions_count from trans_log inner join users on trans_log.user_id = users.user_id where SUBSTR(users.def_acc,3,3) = '" + branchcode + "' group by tran_name";
                //temp query 
                string query = "select distinct trans_log.tran_name as transaction_name  ,sum( trans_log.tran_amount )as  tran_amount,count(trans_log.tran_amount) as transactions_count from trans_log inner join users on  trans_log.user_id = users.user_id group by tran_name";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            result.Add(new TransactionsDetailsModel
                            {
                                transactiontype = sdr["transaction_name"].ToString(),
                                transactioncount = int.Parse(sdr["transactions_count"].ToString()),
                                transactionamount = int.Parse(sdr["tran_amount"].ToString()),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return result;
        }

        public List<TransactionStatusesModel> GetTransactionStatusesDetails(string branchcode)
        {
            List<TransactionStatusesModel> result = new List<TransactionStatusesModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                //string query = "select tran_status as status,count(tran_status) as statuscount from trans_log inner join users on trans_log.user_id = users.user_id where SUBSTR(users.def_acc,3,3) = '" + branchcode + "' group by tran_status";
                //temp query 
                string query = "select tran_status as status,count(tran_status) as statuscount from trans_log inner join users_jsb on trans_log.user_id = users_jsb.user_id group by tran_status";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            result.Add(new TransactionStatusesModel
                            {
                                status = sdr["status"].ToString(),
                                count = int.Parse(sdr["statuscount"].ToString())
                            });
                        }
                    }
                    con.Close();
                }
            }
            return result;
        }

        public Boolean checkadminusernameavailability(string username)
        {
            Boolean result = true;
            int count = 1;

            using (OracleConnection con = new OracleConnection(conString))
            {
                using (OracleCommand cmd = new OracleCommand("select count(user_log) as count from jsb_security_master where user_log = :username"))
                {
                    cmd.Parameters.Add("username", OracleType.VarChar).Value = username;
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            count = int.Parse(sdr["count"].ToString());
                        }
                    }
                    con.Close();
                }
            }

            if (count > 0)
            {
                result = false;
            }
            return result;
        }

        public List<int> GetOnlineOfflineUsers(string branchcode)
        {
            string query;
            List<int> userslist = new List<int>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                if (branchcode == "000")
                {
                    query = "select count(users.user_id) as users from users where users.login_status = 1 union all select count(users.user_id) as users from users where users.login_status = 1 and users.catogry = 1 union all select count(users.user_id) as users from users where users.login_status = 1 and users.catogry = 2 union all select count(users.user_id) as users from users where users.login_status = 1 and users.catogry = 3 union all select count(users.user_id) as users from users where users.login_status = 0 union all select count(users.user_id) as users from users where users.login_status = 0 and users.catogry = 1 union all select count(users.user_id) as users from users where users.login_status = 0 and users.catogry = 2 union all select count(users.user_id) as users from users where users.login_status = 0 and users.catogry = 3";
                }
                else
                {
                    query = "select count(users.user_id) as users from users where users.login_status = 1 and SUBSTR(users.account,3,3) = :b1 union all select count(users.user_id) as users from users where users.login_status = 1 and users.catogry = 1 and SUBSTR(users.account,3,3) = :b2 union all select count(users.user_id) as users from users where users.login_status = 1 and users.catogry = 2 and SUBSTR(users.account,3,3) = :b3 union all select count(users.user_id) as users from users where users.login_status = 1 and users.catogry = 3 and SUBSTR(users.account,3,3) = :b4 union all select count(users.user_id) as users from users where users.login_status = 0 and SUBSTR(users.account,3,3) = :b5 union all select count(users.user_id) as users from users where users.login_status = 0 and users.catogry = 1 and SUBSTR(users.account,3,3) = :b6 union all select count(users.user_id) as users from users where users.login_status = 0 and users.catogry = 2 and SUBSTR(users.account,3,3) = :b7 union all select count(users.user_id) as users from users where users.login_status = 0 and users.catogry = 3 and SUBSTR(users.account,3,3) = :b8";
                }

                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    if (branchcode != "000")
                    {
                        cmd.Parameters.Add("b1", OracleType.VarChar).Value = branchcode;
                        cmd.Parameters.Add("b2", OracleType.VarChar).Value = branchcode;
                        cmd.Parameters.Add("b3", OracleType.VarChar).Value = branchcode;
                        cmd.Parameters.Add("b4", OracleType.VarChar).Value = branchcode;
                        cmd.Parameters.Add("b5", OracleType.VarChar).Value = branchcode;
                        cmd.Parameters.Add("b6", OracleType.VarChar).Value = branchcode;
                        cmd.Parameters.Add("b7", OracleType.VarChar).Value = branchcode;
                        cmd.Parameters.Add("b8", OracleType.VarChar).Value = branchcode;
                    }

                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            userslist.Add(int.Parse(sdr["users"].ToString()));
                        }
                    }
                    con.Close();
                }
            }
            return userslist;
        }

        //-----------------Get User with Branch, Category and Status
        public List<Custreport> GetBranchUsersComplete(string branch, string category, string status, string fromdate, string todate)
        {
            String sqlbranch = "", sqlstatus = "", sqlcategory = "";
            List<Custreport> users = new List<Custreport>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                if (branch != "0" && branch != "000")
                    sqlbranch = " and  substr(account,3,3)='" + branch + "'";

                if (category != "0")
                    sqlcategory = "  and catogry = '" + category + "'";

                if (status != "0" && status != "All")
                    sqlstatus = "  and user_status = '" + status + "'";

                string query = "select user_name,def_acc,branch_name,decode(user_status,'A','Active','B','Blocked','U','Authorized','P','Pending','DE','Deleted','S','Stopped','D','DeActive') as status from users inner join branchs on substr(users.account,3,3) = branchs.branch_code where user_id > 0 and to_date(substr(created_date,0,9),'dd-mon-yy') >= to_date(:fromdate,'mm-dd-yyyy') and to_date(substr(created_date,0,9),'dd-mon-yy') <= to_date(:todate,'mm-dd-yyyy')";

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.Parameters.Add("fromdate", OracleType.VarChar).Value = fromdate;
                cmd.Parameters.Add("todate", OracleType.VarChar).Value = todate;
                if (branch != "0" && branch != "000")
                {
                    query += " and substr(account,3,3)=:branch";
                    cmd.Parameters.Add("branch", OracleType.VarChar).Value = branch;
                }
                if (status != "0" && status != "All")
                {
                    query += " and user_status = :status";
                    cmd.Parameters.Add("status", OracleType.VarChar).Value = status;
                }
                if (category != "0")
                {
                    query += " and catogry = :category";
                    cmd.Parameters.Add("category", OracleType.VarChar).Value = category;
                }
                query += " order by branch_name";
                cmd.CommandText = query;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        users.Add(new Custreport
                        {
                            CustomerName = dr[0].ToString(),
                            AccountNumber = dr[1].ToString(),
                            Branch = dr[2].ToString(),
                            CustStatus = dr[3].ToString()
                        });
                    }
                }
            }
            return users;
        }

        //-----------------Get User with Branch, Category and Status
        public List<Custreport> GetBranchUsers(string branch, string category, string status, string fromdate, string todate)
        {
            String sqlbranch = "", sqlstatus = "", sqlcategory = "" , sqldate = "";
            List<Custreport> users = new List<Custreport>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                if (branch != "0" && branch != "000")
                    sqlbranch = " and branchs.branch_code ='" + branch + "'";

                if (fromdate != "" || todate != "")
                    sqldate = " and to_date(substr(users_jsb.created_date,0,9),'dd-mon-yy') >= to_date('" + fromdate+ "','mm-dd-yyyy') and to_date(substr(users_jsb.created_date,0,9),'dd-mon-yy') <= to_date('" + todate+"','mm-dd-yyyy')";

                if (category != "0")
                    sqlcategory = "  and catogry = '" + category + "'";

                if (status != "0" && status != "All")
                    sqlstatus = "  and users_jsb.user_status = '" + status + "'";

               // string query = "select branch_name_en,count(user_id) as count from users inner join branchs on substr(users.account,3,3) = branchs.branch_code where user_id > 0 and to_date(substr(created_date,0,9),'dd-mon-yy') >= to_date('"+fromdate+"','mm-dd-yyyy') and to_date(substr(created_date,0,9),'dd-mon-yy') <= to_date('"+todate+"','mm-dd-yyyy') "+sqlcategory+" "+sqlbranch+" "+sqlstatus+" group by branch_name_en order by branch_name_en";

                string query = "select distinct branchs.branch_name_en,count(users_jsb.user_id) as count, decode(users_jsb.USER_STATUS,'A','Active','B','Blocked','D','DeActive','P','Pendding','U','Authorized','R','Rejected','DE','Deleted','S','Stopped') as USER_STATUS from users_jsb inner join user_acc_link_jsb on users_jsb.user_log = user_acc_link_jsb.user_id inner join branchs on branchs.branch_code = user_acc_link_jsb.acc_branch where users_jsb.user_id > 0";

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                if (branch != "0" && branch != "000")
                {
                    query += " and branchs.branch_code = :branch";
                    cmd.Parameters.Add("branch", OracleType.VarChar).Value = branch;
                }
                if (fromdate != "" || todate != "")
                {
                    query += " and to_date(substr(users_jsb.created_date,0,9),'dd-mon-yy') >= to_date(:fromdate,'mm-dd-yyyy') and to_date(substr(users_jsb.created_date,0,9),'dd-mon-yy') <= to_date(:todate,'mm-dd-yyyy')";
                    cmd.Parameters.Add("fromdate", OracleType.VarChar).Value = fromdate;
                    cmd.Parameters.Add("todate", OracleType.VarChar).Value = todate;
                }
                if (category != "0")
                {
                    query += " and catogry = :category";
                    cmd.Parameters.Add("category", OracleType.VarChar).Value = category;
                }
                if (status != "0" && status != "All")
                {
                    query += " and users_jsb.user_status = :status";
                    cmd.Parameters.Add("status", OracleType.VarChar).Value = status;
                }
                query += " group by branchs.branch_name_en, users_jsb.user_status order by branchs.branch_name_en";
                cmd.CommandText = query;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        users.Add(new Custreport
                        {
                            Branch = dr[0].ToString(),
                            Count = dr[1].ToString(),
                            CustStatus = dr[2].ToString()
                        });
                    }
                }
            }
            return users;
        }

        public List<userlist> GetAllusers(int roleid)
        {
            List<userlist> users = new List<userlist>();
            string Insql = "";

            using (OracleConnection con = new OracleConnection(conString))
            {

                if (roleid != 1)
                {
                    Insql = " and (u.roleid = " + roleid + " or u.roleidcreated = "+roleid+") ";
                }

                    OracleCommand cmd = new OracleCommand("" +
                     //"SELECT user_name,user_id,r.name,b.branch_name,decode(user_stat,'A','Active','DE','Deleted','D','Deactive') as user_status FROM security_master u , branchs b ,cpanel_rolemaster r where u.roleid=r.roleid and u.user_branch=b.branch_code", con);
                     "SELECT user_name,user_id,r.role_name,b.branch_name_en,decode(user_status,'A','Active','D','Deactive' , 'UA' , 'Un Autherize' , 'RA' , 'Request To Activate' ,'RDA' , 'Request To DeActivate' , 'RED', 'Request To Edit' , 'RD' , 'Request To Delete' , 'R' , 'Rejected' , 'RRP' , 'Request To Reset Password') as user_status  FROM jsb_security_master u , branchs b ,jsb_roles_master r where u.roleid=r.role_id and u.user_branch=b.branch_code  " + Insql + "  and user_status <> 'DE'", con);


               // "SELECT user_name,user_id,r.role_name,b.branch_name_en,decode(user_status,'A','Active','DE','Deleted','D','Deactive' , 'UA' , 'Un Autherize' , 'RA' , 'Request To Activate' ,'RDA' , 'Request To DeActivate' , 'RED', 'Request To Edit' , 'RD' , 'Request To Delete' , 'R' , 'Rejectd' , 'RRP' , 'Request To Reset Password') as user_status  FROM jsb_security_master u , branchs b ,jsb_roles_master r where u.roleid=r.role_id and u.user_branch=b.branch_code and user_status = 'A'  " + Insql + "", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {


                        users.Add(new userlist
                        {
                            name = dr[0].ToString(),
                            user_id = Convert.ToInt32(dr[1].ToString()),
                            user_branch = dr[3].ToString(),
                            rolename = dr[2].ToString(),
                            user_status = dr[4].ToString()
                        });
                    }
                }


            }
            return users;
        }

        public List<userlist> GetAllPenddingusers(int roleid)
        {
            List<userlist> users = new List<userlist>();
            string Insql = "";

            using (OracleConnection con = new OracleConnection(conString))
            {

                if (roleid != 1)
                {
                    Insql = " and (u.roleid = " + roleid + " or u.roleidcreated = " + roleid + " )  ";

                    //Insql = " and (u.roleid = " + roleid + "   ";
                }

                OracleCommand cmd = new OracleCommand("" +


                    //"SELECT user_name,user_id,r.name,b.branch_name,decode(user_stat,'A','Active','DE','Deleted','D','Deactive') as user_status FROM security_master u , branchs b ,cpanel_rolemaster r where u.roleid=r.roleid and u.user_branch=b.branch_code", con);

                    "SELECT user_name,user_id,r.role_name,b.branch_name_en, user_status as user_status_code ,decode(user_status , 'UA' , 'Un Autherize' , 'RA' , 'Request To Activate' ,'RDA' , 'Request To DeActivate' , 'RED', 'Request To Edit' , 'RD' , 'Request To Delete'  , 'RRP' , 'Request To Reset') as user_status   FROM jsb_security_master u , branchs b ,jsb_roles_master r where u.roleid=r.role_id and u.user_branch=b.branch_code and user_status <> 'A'  and user_status <> 'D'  and user_status <> 'DE' and user_status <> 'R' ", con);  //" + Insql + "



                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {


                        users.Add(new userlist
                        {
                            name = dr[0].ToString(),
                            user_id = Convert.ToInt32(dr[1].ToString()),
                            user_branch = dr[3].ToString(),
                            rolename = dr[2].ToString(),
                            user_status_code = dr[4].ToString(),
                            user_status = dr[5].ToString()
                        });
                    }
                }


            }
            return users;
        }

        public List<profilelist> GetAllProfiles()
        {
            List<profilelist> profiles = new List<profilelist>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select jsb_roles_master.role_id,jsb_roles_master.role_name,jsb_roles_master.role_creation_date,count(jsb_security_master.user_log)as usercount from jsb_roles_master left outer join jsb_security_master  on jsb_roles_master.role_id = jsb_security_master.roleid group by jsb_roles_master.role_id,jsb_roles_master.role_name,jsb_roles_master.role_creation_date", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {


                        profiles.Add(new profilelist
                        {
                            name = dr[1].ToString(),
                            role_id = Convert.ToInt32(dr[0].ToString()),
                            inserted_date = dr[3].ToString(),
                            users_count = dr[2].ToString()

                        });
                    }
                }


            }
            return profiles;
        }

        public List<profilelist> GetAllCustomerProfiles()
        {
            List<profilelist> profiles = new List<profilelist>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select tbl_rolemaster.name,tbl_rolemaster.roleid,count(users.user_log)as usercount from tbl_rolemaster left outer join users on tbl_rolemaster.roleid = users.roleid group by tbl_rolemaster.roleid,tbl_rolemaster.name", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {


                        profiles.Add(new profilelist
                        {
                            name = dr[0].ToString(),
                            role_id = Convert.ToInt32(dr[1].ToString()),
                            //inserted_date = dr[3].ToString(),
                            users_count = dr[2].ToString()

                        });
                    }
                }


            }
            return profiles;
        }

        public int getcpanelprofileuserscount(int roleid)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select count(user_log) as userscount from jsb_security_master where roleid = :roleid", con);
                cmd.Parameters.Add("roleid", OracleType.Int32).Value = roleid;
                int count = 0;
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    OracleDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        count = int.Parse(dr[0].ToString());
                    }
                }
                return count;
            }
        }

        public int deletecpanelprofile(int roleid)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("delete from jsb_roles_master where role_id = :roleid", con);
                cmd.Parameters.Add("roleid", OracleType.Int32).Value = roleid;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        public int resetpassworduserA(int user_id, string p)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update jsb_security_master set USER_LAST_LOGIN='F', USER_STATUS='A', USER_PWD=:pwd where user_id=:user_id", con);
                cmd.Parameters.Add("pwd", OracleType.VarChar).Value = p;
                cmd.Parameters.Add("user_id", OracleType.Int32).Value = user_id;

                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }
        public int resetpassworduser(int user_id)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update jsb_security_master set USER_LAST_LOGIN='F', USER_STATUS='RRP' where user_id=:user_id", con);
                cmd.Parameters.Add("user_id", OracleType.Int32).Value = user_id;

                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }


        public string getinfo(int user_id)
        {
           


            OracleCommand cmd;
            OracleDataReader dr;

            string result = "0";
            string query1;
               // List<userlist> info = new List<userlist>();
            query1 = "select USER_MOBILE from jsb_security_master where user_id = :user_id";

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);
                cmd.Parameters.Add("user_id", OracleType.Int32).Value = user_id;

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        //info.Add(new userlist
                        //{

                        result = dr[0].ToString();
                         



                        //});
                            }
                    }
                
            }


           


            return result;



        }

        public List<userlist> getMoreinfo(int user_id)
        {



            OracleCommand cmd;
            OracleDataReader dr;

            string result = "0";
            string query1;
             List<userlist> info = new List<userlist>();
            query1 = "select user_log, user_name, user_mobile from jsb_security_master where user_id = :user_id";

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);
                cmd.Parameters.Add("user_id", OracleType.Int32).Value = user_id;

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        info.Add(new userlist
                        {

                        user_log = dr[0].ToString(),
                         user_name = dr[1].ToString(),
                         user_mobile = dr[2].ToString()




                        });
                    }
                }

            }





            return info;



        }
        public int Authresetpassworduser(int user_id)
        {
            string p = CreatePassword(8);

            // WAPT11: store a one-way hash rather than reversible ciphertext.
            string enc_pwd = AljazeeraCPanel.Security.PasswordHasher.Hash(p);
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update jsb_security_master set USER_LAST_LOGIN='F', USER_STATUS='A', USER_PWD=:enc_pwd where user_id=:user_id", con);
                cmd.Parameters.Add("enc_pwd", OracleType.VarChar).Value = enc_pwd;
                cmd.Parameters.Add("user_id", OracleType.Int32).Value = user_id;

                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        public int UpdateuserSTS(int user_id, string sts)
        {
            string p = CreatePassword(8);

            string enc_pwd = Encrypt(p);
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update jsb_security_master set USER_STATUS = :sts where user_id = :user_id", con);
                cmd.Parameters.Add("sts", OracleType.VarChar).Value = sts;
                cmd.Parameters.Add("user_id", OracleType.Int32).Value = user_id;

                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        public int deactive(int user_id)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update jsb_security_master set user_status='RDA' where user_id=:user_id", con);
                cmd.Parameters.Add("user_id", OracleType.Int32).Value = user_id;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        public int Active(int user_id)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update jsb_security_master set user_status='RA' where user_id=:user_id", con);
                cmd.Parameters.Add("user_id", OracleType.Int32).Value = user_id;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        public int deleteuser(int user_id)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update jsb_security_master set user_status='RD' where user_id = :user_id", con);
                cmd.Parameters.Add("user_id", OracleType.Int32).Value = user_id;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }


        public int Authdeactive(int user_id)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update jsb_security_master set user_status='D' where user_id=:user_id", con);
                cmd.Parameters.Add("user_id", OracleType.Int32).Value = user_id;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        public int AuthActive(int user_id)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update jsb_security_master set user_status='A' where user_id=:user_id", con);
                cmd.Parameters.Add("user_id", OracleType.Int32).Value = user_id;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        public int Authdeleteuser(int user_id)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update jsb_security_master set user_status='DE' where user_id = :user_id", con);
                cmd.Parameters.Add("user_id", OracleType.Int32).Value = user_id;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        //public int deleteuser(int user_id)
        //{
        //    using (OracleConnection con = new OracleConnection(conString))
        //    {
        //        //OracleCommand cmd = new OracleCommand("Update security_master set USER_STAT ='DE' where  user_id='" + user_id + "'", con);
        //        OracleCommand cmd = new OracleCommand("delete from jsb_security_master where  user_id = '" + user_id + "'", con);
        //        if (con.State == ConnectionState.Closed)
        //        { con.Open(); }

        //        return cmd.ExecuteNonQuery();
        //    }
        //}

        public int deleteservice(int service_id)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update SERVICES set service_status='DE' where service_id=:service_id", con);
                cmd.Parameters.Add("service_id", OracleType.Int32).Value = service_id;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }
        public List<ChqRequest> Chqrequest(String bracode)
        {
            OracleCommand cmd;
            OracleDataReader dr;

            int requestid;
            String name, act, date, booksize, reqdate , userid,reqsts,branchcode,branchname;
            String query1, result;
            List<ChqRequest> customer = new List<ChqRequest>();

            query1 = " select req_id,user_id,req_creation_date,req_status,acc_no,receiving_branch,branch_name_en,req_type_name_en from jsb_user_service_reqs inner join jsb_service_req_types on jsb_user_service_reqs.req_type = jsb_service_req_types.req_type_id inner join branchs on jsb_user_service_reqs.receiving_branch = branchs.branch_code and ( jsb_service_req_types.req_type_id = '1001' or jsb_service_req_types.req_type_id = '1002') and jsb_user_service_reqs.receiving_branch = :bracode ";

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);
                cmd.Parameters.Add("bracode", OracleType.VarChar).Value = bracode;

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        requestid = Convert.ToInt32(dr[0].ToString());
                        userid = dr[1].ToString();
                        reqdate = dr[2].ToString();
                        reqsts = dr[3].ToString();
                        act = dr[4].ToString();
                        branchcode = dr[5].ToString();
                        branchname = dr[6].ToString();
                        booksize = dr[7].ToString();
                        //date = dr[3].ToString();

                        //    name = dr[4].ToString();
                        if (reqsts == "P")
                        {
                            reqsts = "Pendding";

                        }

                        if (reqsts == "A")
                        {
                            reqsts = "Accept";

                        }

                        if (reqsts == "R")
                        {
                            reqsts = "Reject";

                        }

                        customer.Add(new ChqRequest
                        {
                            request_id = requestid,
                            userid = userid,
                            reqdate = reqdate,
                            reqsts = reqsts,
                            act = act,
                            branchcode = branchcode,
                            branchname = branchname,
                            booksize = booksize



                        });
                    }
                }


            }


            return customer;

        }

        public List<ChqRequest> Cardrequest(String bracode)
        {
            OracleCommand cmd;
            OracleDataReader dr;

            int requestid;
            String name, act, date, booksize, reqdate, userid, reqsts, branchcode, branchname;
            String query1, result;
            List<ChqRequest> customer = new List<ChqRequest>();
            query1 = " select req_id,user_id,req_creation_date,req_status,acc_no,receiving_branch,branch_name_en,req_type_name_en from jsb_user_service_reqs inner join jsb_service_req_types on jsb_user_service_reqs.req_type = jsb_service_req_types.req_type_id inner join branchs on jsb_user_service_reqs.receiving_branch = branchs.branch_code and ( jsb_service_req_types.req_type_id = '2001' or jsb_service_req_types.req_type_id = '2002') and jsb_user_service_reqs.receiving_branch = :bracode ";

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);
                cmd.Parameters.Add("bracode", OracleType.VarChar).Value = bracode;

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        requestid = Convert.ToInt32(dr[0].ToString());
                        userid = dr[1].ToString();
                        reqdate = dr[2].ToString();
                        reqsts = dr[3].ToString();
                        act = dr[4].ToString();
                        branchcode = dr[5].ToString();
                        branchname = dr[6].ToString();
                        booksize = dr[7].ToString();
                        //date = dr[3].ToString();

                        //    name = dr[4].ToString();
                        if (reqsts == "P")
                        {
                            reqsts = "Pendding";

                        }

                        if (reqsts == "A")
                        {
                            reqsts = "Accept";

                        }

                        if (reqsts == "R")
                        {
                            reqsts = "Reject";

                        }

                        customer.Add(new ChqRequest
                        {
                            request_id = requestid,
                            userid = userid,
                            reqdate = reqdate,
                            reqsts = reqsts,
                            act = act,
                            branchcode = branchcode,
                            branchname = branchname,
                            booksize = booksize



                        });
                    }
                }


            }


            return customer;

        }

        public List<ChqRequest> ChqrequestReport(String bracode)
        {
            OracleCommand cmd;
            OracleDataReader dr;

            int requestid;
            String name, act, date, booksize, reqdate, status;
            String query1, result;
            List<ChqRequest> customer = new List<ChqRequest>();
            if (!bracode.Equals("000"))
            {
                query1 = "select users.user_name||' - '||SUBSTR(cheque_reqs.account_no,14,7) as customer,cheque_reqs.requested_size,cheque_reqs.req_date,cheque_reqs.req_status from cheque_reqs inner join users on users.user_id = cheque_reqs.user_id where cheque_reqs.req_status <> 'process' and SUBSTR(cheque_reqs.account_no,3,3) = :bracode";
            }
            else
            {
                query1 = "select users.user_name||' - '||SUBSTR(cheque_reqs.account_no,14,7) as customer,cheque_reqs.requested_size,cheque_reqs.req_date,cheque_reqs.req_status from cheque_reqs inner join users on users.user_id = cheque_reqs.user_id where cheque_reqs.req_status <> 'process'";
            }

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);
                if (!bracode.Equals("000"))
                {
                    cmd.Parameters.Add("bracode", OracleType.VarChar).Value = bracode;
                }

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        name = dr[0].ToString();
                        booksize = dr[1].ToString();
                        date = dr[2].ToString();
                        status = dr[3].ToString();

                        customer.Add(new ChqRequest
                        {
                            accountmap = name,
                            booksize = booksize,
                            name = name,
                            date = date,
                            status = status
                        });
                    }
                }
            }
            return customer;

        }

        public List<AtmCardModel> GetCardsRequests(String bracode)
        {
            OracleCommand cmd;
            OracleDataReader dr;
            int requestid;
            String name, act, date, booksize, reqdate, nameoncard, reason;
            String query1, result;
            List<AtmCardModel> card = new List<AtmCardModel>();
            if (!bracode.Equals("000"))
            {
                //query1 = "select c.request_id,branch_name||'-'||curr_name||'-'||act_name||'-'|| SUBSTR(c.account_no,14) account_no,c.requested_size,c.req_date,u.user_name from cheque_reqs c,users u,branchs, currency,act_types where req_status='process' and u.user_id=c.user_id and   SUBSTR(c.account_no,3,3)='" + bracode + "' and branchs.branch_code=SUBSTR(c.account_no,3,3) and act_types.act_type_code=SUBSTR(c.account_no,6,5) and  currency.CURR_STS='1' and  currency.curr_code=SUBSTR(c.account_no,11,3) order by c.request_id";
                query1 = "select c.request_id,branch_name||'-'||curr_name||'-'||act_name||'-'|| SUBSTR(c.account_no,14) account_no,c.name_on_card,c.req_date,c.req_reason,u.user_name from card_reqs c,users u,branchs, currency,act_types where req_status='process' and u.user_id=c.user_id and branchs.branch_code=SUBSTR(c.account_no,3,3) and act_types.act_type_code=SUBSTR(c.account_no,6,5) and currency.CURR_STS='1' and currency.curr_code=SUBSTR(c.account_no,11,3) and SUBSTR(c.account_no,3,3)='" + bracode + "' order by c.request_id";
            }
            else
            {
                query1 = "select c.request_id,branch_name||'-'||curr_name||'-'||act_name||'-'|| SUBSTR(c.account_no,14) account_no,c.name_on_card,c.req_date,c.req_reason,u.user_name from card_reqs c,users u,branchs, currency,act_types where req_status='process' and u.user_id=c.user_id and branchs.branch_code=SUBSTR(c.account_no,3,3) and act_types.act_type_code=SUBSTR(c.account_no,6,5) and currency.CURR_STS='1' and currency.curr_code=SUBSTR(c.account_no,11,3) order by c.request_id";
            }
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        requestid = Convert.ToInt32(dr[0].ToString());
                        act = dr[1].ToString();
                        nameoncard = dr[2].ToString();
                        date = dr[3].ToString();
                        reason = dr[4].ToString();
                        name = dr[5].ToString();


                        card.Add(new AtmCardModel
                        {
                            request_id = requestid.ToString(),
                            account_number = act,
                            name = name,
                            request_date = date,
                            name_on_card = nameoncard,
                            request_reason = reason
                        });
                    }
                }
            }
            return card;
        }

        public List<AtmCardModel> AtmCardsReport(String bracode)
        {
            OracleCommand cmd;
            OracleDataReader dr;
            int requestid;
            String name, act, date, booksize, reqdate, nameoncard, reason, status;
            String query1, result;
            List<AtmCardModel> card = new List<AtmCardModel>();
            if (!bracode.Equals("000"))
            {
                //query1 = "select c.request_id,branch_name||'-'||curr_name||'-'||act_name||'-'|| SUBSTR(c.account_no,14) account_no,c.requested_size,c.req_date,u.user_name from cheque_reqs c,users u,branchs, currency,act_types where req_status='process' and u.user_id=c.user_id and   SUBSTR(c.account_no,3,3)='" + bracode + "' and branchs.branch_code=SUBSTR(c.account_no,3,3) and act_types.act_type_code=SUBSTR(c.account_no,6,5) and  currency.CURR_STS='1' and  currency.curr_code=SUBSTR(c.account_no,11,3) order by c.request_id";
                query1 = "select users.user_name||' - '||SUBSTR(card_reqs.account_no,14,7) as customer,card_reqs.req_date,card_reqs.req_status,card_reqs.req_reason,card_reqs.name_on_card from card_reqs inner join users on users.user_id = card_reqs.user_id where card_reqs.req_status <> 'process' and SUBSTR(card_reqs.account_no,3,3) = :bracode";
            }
            else
            {
                query1 = "select users.user_name||' - '||SUBSTR(card_reqs.account_no,14,7) as customer,card_reqs.req_date,card_reqs.req_status,card_reqs.req_reason,card_reqs.name_on_card from card_reqs inner join users on users.user_id = card_reqs.user_id where card_reqs.req_status <> 'process'";
            }
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);
                if (!bracode.Equals("000"))
                {
                    cmd.Parameters.Add("bracode", OracleType.VarChar).Value = bracode;
                }

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        name = dr[0].ToString();
                        date = dr[1].ToString();
                        status = dr[2].ToString();
                        reason = dr[3].ToString();
                        nameoncard = dr[4].ToString();

                        card.Add(new AtmCardModel
                        {
                            name = name,
                            request_date = date,
                            name_on_card = nameoncard,
                            request_reason = reason,
                            request_status = status
                        });
                    }
                }
            }
            return card;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public String getaccount(string user_id)
        {
            String Accounts = "";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "SELECT acc_id,acc_no from user_acc_link where user_id = :user_id";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("user_id", OracleType.VarChar).Value = user_id;

                con.Open();


                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {

                        if (dataReader["acc_id"] != DBNull.Value)
                        {

                            if (dataReader["acc_no"] != DBNull.Value)
                            {
                                Accounts = Accounts + "-" + (string)dataReader["acc_no"];
                                //Accounts = Accounts.Substring(2);
                            }

                        }
                    }
                    Accounts = Accounts.Substring(1);
                    return Accounts;

                }

            }

        }
        //---------------------------------------------------------get act --------------------------------//
        public String getspfaccount(string user_id, string act)
        {
            String Accounts = "";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "SELECT acc_id,acc_no from user_acc_link where user_id = :user_id and substr(acc_no,14) = :act";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("user_id", OracleType.VarChar).Value = user_id;
                cmd.Parameters.Add("act", OracleType.VarChar).Value = act;

                con.Open();


                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {

                        if (dataReader["acc_id"] != DBNull.Value)
                        {

                            if (dataReader["acc_no"] != DBNull.Value)
                            {
                                Accounts = (string)dataReader["acc_no"];
                                //Accounts = Accounts.Substring(2);
                            }

                        }
                    }

                    return Accounts;

                }

            }

        }

        //-----------------------DropDownGET Branchs------------------------------------------------------
        //
        public List<CustomerRegBankinfo> GetBranchs()
        {
            string branchs = "";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select branch_code,branch_name from branchs where branch_sts = '1'";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();

                List<CustomerRegBankinfo> list = new List<CustomerRegBankinfo>();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        CustomerRegBankinfo obj = new CustomerRegBankinfo();

                        if (dataReader["branch_code"] != DBNull.Value)
                        {
                            obj.BranchCode = (string)dataReader["branch_code"];

                            if (dataReader["branch_name"] != DBNull.Value)
                            {
                                obj.Branch = (string)dataReader["branch_name"];

                            }

                            list.Add(obj);

                        }
                    }
                    //branchs = branchs.Substring(1);
                    return list;
                }
            }
        }

        //----------------------DropDownGet Account Type---------------------------------
        public List<CustomerRegBankinfo> GetAccountType()
        {
            string branchs = "";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select act_type_code,act_name from Act_types";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();

                List<CustomerRegBankinfo> list = new List<CustomerRegBankinfo>();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        CustomerRegBankinfo obj = new CustomerRegBankinfo();

                        if (dataReader["act_type_code"] != DBNull.Value)
                        {
                            obj.AccountTypecode = (string)dataReader["act_type_code"];

                            if (dataReader["act_name"] != DBNull.Value)
                            {
                                obj.AccountType = (string)dataReader["act_name"];

                            }

                            list.Add(obj);

                        }
                    }
                    //branchs = branchs.Substring(1);
                    return list;
                }
            }
        }

        //------------------DropDown Get Currency---------------------------
        public List<CustomerRegBankinfo> GetCurrency()
        {
            string branchs = "";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select curr_code,curr_name from currency where CURR_STS='1' ";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();

                List<CustomerRegBankinfo> list = new List<CustomerRegBankinfo>();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        CustomerRegBankinfo obj = new CustomerRegBankinfo();

                        if (dataReader["curr_code"] != DBNull.Value)
                        {
                            obj.CurrencyCode = (string)dataReader["curr_code"];

                            if (dataReader["curr_name"] != DBNull.Value)
                            {
                                obj.Currency = (string)dataReader["curr_name"];

                            }

                            list.Add(obj);

                        }
                    }
                    //branchs = branchs.Substring(1);
                    return list;
                }
            }
        }


        //-----------------------GET AccountTypes------------------------------------------------------
        //
        public String getaccounttype(string acctype)
        {
            String acctypename = "NULL";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select act_name from act_types where act_type_code = :acctype";
                string query2 = "select act_name from invact_types where act_type_code = :acctype";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("acctype", OracleType.VarChar).Value = acctype;
                OracleCommand cmd2 = new OracleCommand(query2, con);
                cmd2.Parameters.Add("acctype", OracleType.VarChar).Value = acctype;
                con.Open();


                OracleDataReader dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        if (dataReader["act_name"] != DBNull.Value)
                        {
                            acctypename = (string)dataReader["act_name"];
                        }

                    }
                }
                else
                {
                    dataReader = cmd2.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            if (dataReader["act_name"] != DBNull.Value)
                            {
                                acctypename = (string)dataReader["act_name"];
                            }

                        }
                    }
                    else
                    { acctypename = "Account Type Not Found"; }
                }



                return acctypename;

            }

        }


        //-----------------------GET BRANCH NAME English------------------------------------------------------
        //
        public String getbranchnameenglish(string brcode)
        {
            String brname = "NULL";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select branch_name_en from branchs where branch_code = :brcode";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("brcode", OracleType.VarChar).Value = brcode;

                con.Open();


                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {



                        if (dataReader["branch_name_en"] != DBNull.Value)
                        {
                            brname = (string)dataReader["branch_name_en"];
                        }

                    }
                }
                if (brcode == "000")
                {
                    return "Admin";
                }
                return brname;

            }

        }


        public string getFromAccInfo(string account)
        {
            
            String account_info = "NULL";

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select branch_name||'-'||act_name||'-'|| SUBSTR(:account,14,11) account_info from users u,branchs,act_types where branchs.branch_code=SUBSTR(:account,3,3) and act_types.act_type_code=SUBSTR(:account,6,5)";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("account", OracleType.VarChar).Value = account;

                con.Open();
                using (IDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {


                        if (dr["account_info"] != DBNull.Value)
                        {
                            account_info = (string)dr["account_info"];
                        }

                      

                    }
                    
                }
                return account_info;

            }

        }

        public String getcurrencyname(string currcode)
        {
            String curr_name = "NULL";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select curr_name from currency where CURR_STS='1' and curr_code = :currcode";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("currcode", OracleType.VarChar).Value = currcode;

                con.Open();


                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {



                        if (dataReader["curr_name"] != DBNull.Value)
                        {
                            curr_name = (string)dataReader["curr_name"];
                        }

                    }
                }

                return curr_name;

            }

        }



        //-------------------------------------DropClient for ChequeStatus Controller DropDownList--------------------------------
        //
        public List<CustomerRegBankinfo> DropClient(string user_id)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " SELECT acc_id,acc_no from user_acc_link where user_id =" + user_id;

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();

                List<CustomerRegBankinfo> list = new List<CustomerRegBankinfo>();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        CustomerRegBankinfo obj = new CustomerRegBankinfo();
                        if (dataReader["acc_id"] != DBNull.Value)
                        {
                            if (dataReader["acc_id"] != DBNull.Value)
                            {
                                //obj.AccountID = (int)dataReader["acc_id"];
                                obj.CustomerID = dataReader["acc_id"].ToString();
                            }
                            if (dataReader["acc_no"] != DBNull.Value)
                            {
                                obj.AccountNumber = (string)dataReader["acc_no"];
                            }
                            list.Add(obj);
                        }
                    }

                    return list;

                }

            }

        }


        public List<CustomerRegBankinfo> checkaccount(string act)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select user_id,user_name from users where DEF_ACC='" + act + "'";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();

                List<CustomerRegBankinfo> list = new List<CustomerRegBankinfo>();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        CustomerRegBankinfo obj = new CustomerRegBankinfo();
                        if (dataReader["user_id"] != DBNull.Value)
                        {
                            if (dataReader["user_id"] != DBNull.Value)
                            {
                                //obj.AccountID = (int)dataReader["acc_id"];
                                obj.CustomerID = dataReader["user_id"].ToString();
                            }
                            if (dataReader["user_name"] != DBNull.Value)
                            {
                                obj.CustomerName = (string)dataReader["user_name"];
                            }
                            list.Add(obj);
                        }
                    }

                    return list;

                }

            }

        }


        //---------------------test pr--------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="FILE_NAME"></param>
        /// <returns></returns>
        public int insertfilesalary(string user_id, string FILE_NAME)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "INSERT INTO salary_files (FILE_ID,FILE_NAME,NO_OF_ROWS,STATUS,NO_OF_PROCESS_ROWS,FILE_DATE,USER_ID,FILE_TOTAL) VALUES(salaryfile.nextval,:file_name,'0','P','0',sysdate,:user_id,'0')";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("file_name", OracleType.VarChar).Value = FILE_NAME;
                cmd.Parameters.Add("user_id", OracleType.VarChar).Value = user_id;

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();



                return result;

            }



        }

        //----------------------- INSERT FiLE SALARY ITEMS----------------------------------------------
        //
        public int insertfilesalaryitems(string user_id, string FILE_NAME, string acc, string amount, string acccomp)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "INSERT INTO salary_temp (SALARY_ID,SALARY_USER_ID,SALARY_ACCOUNT_NO,SALARY_AMOUNT,SALARY_STATUS,SALARY_FILE_NAME,SALARY_COMP_ACT,SALARY_PROCESS_DATE,SALARY_REQ_DATE) VALUES(salarytemp.nextval,:user_id,:acc,:amount,'P',:file_name,:acccomp,sysdate,sysdate)";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("user_id", OracleType.VarChar).Value = user_id;
                cmd.Parameters.Add("acc", OracleType.VarChar).Value = acc;
                cmd.Parameters.Add("amount", OracleType.VarChar).Value = amount;
                cmd.Parameters.Add("file_name", OracleType.VarChar).Value = FILE_NAME;
                cmd.Parameters.Add("acccomp", OracleType.VarChar).Value = acccomp;

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();



                return result;

            }



        }

        //----------------------- update FiLE SALARY ITEMS----------------------------------------------
        //
        public int updatesalaryitems(string user_id, string FILE_NAME, string acc, string sts)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "update salary_temp set SALARY_STATUS = :sts, SALARY_PROCESS_DATE=sysdate where SALARY_USER_ID = :user_id and SALARY_ACCOUNT_NO = :acc and SALARY_FILE_NAME = :file_name";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("sts", OracleType.VarChar).Value = sts;
                cmd.Parameters.Add("user_id", OracleType.VarChar).Value = user_id;
                cmd.Parameters.Add("acc", OracleType.VarChar).Value = acc;
                cmd.Parameters.Add("file_name", OracleType.VarChar).Value = FILE_NAME;

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();



                return result;

            }



        }


        /// updates file sallary
        /// items in a table
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileName"></param>
        /// <param name="countrow"></param>
        /// <param name="totalamount"></param>
        /// <param name="modelAccountNumber"></param>
        /// <returns></returns>
        /// //-----------------------------------updatefilesalaryitems---------------------
        public int updatefilesalaryitems(string userId, string fileName, int countrow, double totalamount, string modelAccountNumber)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "update salary_files set NO_OF_ROWS = :countrow, STATUS='RWS', FILE_TOTAL = :totalamount where FILE_NAME = :fileName and user_id = :userId";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("countrow", OracleType.Int32).Value = countrow;
                cmd.Parameters.Add("totalamount", OracleType.Double).Value = totalamount;
                cmd.Parameters.Add("fileName", OracleType.VarChar).Value = fileName;
                cmd.Parameters.Add("userId", OracleType.VarChar).Value = userId;

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();



                return result;

            }
        }


        //-----------------------------------InsertTranLog---------------------
        /// <summary>
        /// Insert into Log 
        /// all the info about each transaction
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="tranName"></param>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        /// <param name="status"></param>
        /// <param name="respResult"></param>
        /// <returns></returns>
        public int InsertTranLog(string user_id, string tranName, string req, string resp, string status, string respResult)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "INSERT INTO trans_log (TRAN_ID,TRAN_REQ,TRAN_RESP,TRAN_REQ_DATE,TRAN_RESP_DATE,TRAN_STATUS,TRAN_RESP_RESULT,USER_ID,TRAN_NAME) VALUES(tranlog.nextval,:req,:resp,sysdate,sysdate,:status,:respResult,:user_id,:tranName)";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("req", OracleType.VarChar).Value = req;
                cmd.Parameters.Add("resp", OracleType.VarChar).Value = resp;
                cmd.Parameters.Add("status", OracleType.VarChar).Value = status;
                cmd.Parameters.Add("respResult", OracleType.VarChar).Value = respResult;
                cmd.Parameters.Add("user_id", OracleType.VarChar).Value = user_id;
                cmd.Parameters.Add("tranName", OracleType.VarChar).Value = tranName;

                con.Open();
                int result = -1;

                result = cmd.ExecuteNonQuery();



                return result;

            }
        }


        public int InsertChequeReq(string user_id, string accountNo, string size)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "INSERT INTO cheque_reqs (REQUEST_ID,ACCOUNT_NO,USER_ID,REQUESTED_SIZE,REQ_DATE,REQ_STATUS,REQ_REASON) VALUES(cheque_req_seq.nextval,:accountNo,:user_id,:size,sysdate,'process','')";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("accountNo", OracleType.VarChar).Value = accountNo;
                cmd.Parameters.Add("user_id", OracleType.VarChar).Value = user_id;
                cmd.Parameters.Add("size", OracleType.VarChar).Value = size;

                con.Open();
                int result = -1;

                result = cmd.ExecuteNonQuery();



                return result;

            }
        }



        public String custregcheck(String acc_no, String rim)
        {
            String lblconfirm;

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select count(*) from users_jsb where user_rim = :rim", con);
                cmd.Parameters.Add("rim", OracleType.VarChar).Value = rim;

                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                dr.Read();

                int counter = Convert.ToInt32(dr[0].ToString());
                dr.Close();

                if (counter == 0)
                {
                    cmd = new OracleCommand("select count(*) from user_acc_link_jsb where acc_no = :acc_no", con);
                    cmd.Parameters.Add("acc_no", OracleType.VarChar).Value = acc_no;

                    dr = cmd.ExecuteReader();
                    dr.Read();

                    counter = Convert.ToInt32(dr[0].ToString());
                    dr.Close();
                    if (counter != 0)
                    {
                        lblconfirm = "This Account is linked with another user";
                        return lblconfirm;
                    }

                    lblconfirm = "This Account is available";
                }
                else
                {
                    lblconfirm = "This Account is Already exist";
                }
            }
            return lblconfirm;
        }

        public String custregcheckperaddlink(String acc_no, String rim)
        {
            Boolean FLAG;
            String lblconfirm;
            OracleCommand cmd;
            OracleDataReader dr;
            int counter;

            //String query1 = "select count(*) from users_jsb  where DEF_ACC='" + acc_no + "'";
            String query1 = "select count(*) from users_jsb  where user_rim ='" + rim + "'";
            String query2 = "select count(*) from user_acc_link_jsb where acc_no='" + acc_no + "'";

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();

                dr = cmd.ExecuteReader();
                dr.Read();

                counter = Convert.ToInt32(dr[0].ToString());
                dr.Close();
                con.Close();
                if ((counter != 0))
                {

                    cmd = new OracleCommand(query2, con);

                    con.Open();

                    dr = cmd.ExecuteReader();
                    dr.Read();

                    counter = Convert.ToInt32(dr[0].ToString());
                    dr.Close();
                    con.Close();
                    if ((counter != 0))
                    {
                        lblconfirm = "This Account is linked with another user";

                        return lblconfirm;
                    }
                    else
                    {
                        lblconfirm = "This Account is available";
                    }
                }
                else
                {
                    lblconfirm = "This user  Account is not exist";
                }

            }
            return lblconfirm;
        }

        public String custregcheckforlink(String acc_no, String rim , string userlog)
        {
            Boolean FLAG;
            String lblconfirm;
            OracleCommand cmd;
            OracleDataReader dr;
            int counter;

            //String query1 = "select count(*) from users_jsb  where DEF_ACC='" + acc_no + "'";
            String query1 = "select count(*) from users_jsb  where user_rim ='" + rim + "' and user_log = '"+userlog+"'";
            String query2 = "select count(*) from user_acc_link_jsb where acc_no='" + acc_no + "'  and user_id = '"+userlog+"'";

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();

                dr = cmd.ExecuteReader();
                dr.Read();

                counter = Convert.ToInt32(dr[0].ToString());
                dr.Close();
                con.Close();
                if ((counter != 0))
                {

                    cmd = new OracleCommand(query2, con);

                    con.Open();

                    dr = cmd.ExecuteReader();
                    dr.Read();

                    counter = Convert.ToInt32(dr[0].ToString());
                    dr.Close();
                    con.Close();

                    if ((counter != 0))
                    {
                        lblconfirm = "This Account is linked with another user";

                        return lblconfirm;
                    }
                    else
                    {
                        lblconfirm = "This Account is available";
                    }
                }
                else
                {
                    lblconfirm = "This user Account is not exist";
                }

            }
            return lblconfirm;
        }


        public String custregcheckforlinkreg(String acc_no, String rim, string userlog)
        {
            String lblconfirm;

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select count(*) from users_jsb where user_rim = :rim and user_log = :userlog", con);
                cmd.Parameters.Add("rim", OracleType.VarChar).Value = rim;
                cmd.Parameters.Add("userlog", OracleType.VarChar).Value = userlog;

                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                dr.Read();

                int counter = Convert.ToInt32(dr[0].ToString());
                dr.Close();
                if (counter == 0)
                {
                    cmd = new OracleCommand("select count(*) from user_acc_link_jsb where acc_no = :acc_no and user_id = :userlog", con);
                    cmd.Parameters.Add("acc_no", OracleType.VarChar).Value = acc_no;
                    cmd.Parameters.Add("userlog", OracleType.VarChar).Value = userlog;

                    dr = cmd.ExecuteReader();
                    dr.Read();

                    counter = Convert.ToInt32(dr[0].ToString());
                    dr.Close();

                    if (counter != 0)
                    {
                        lblconfirm = "This Account is linked with another user";
                        return lblconfirm;
                    }

                    lblconfirm = "This Account is available";
                }
                else
                {
                    lblconfirm = "This user  is already exist";
                }
            }
            return lblconfirm;
        }


        public String custregcheck2(String Account, String category)
        {
            Boolean FLAG;
            String lblconfirm;
            OracleCommand cmd;
            OracleDataReader dr;
            int counter;
            if (String.IsNullOrEmpty(category)){
                category = "1";
            }
            String query1 = "select count(*) from users_jsb  where def_acc = '" + Account + "' and roleid = '"+category+"'  ";

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();

                dr = cmd.ExecuteReader();
                dr.Read();

                counter = Convert.ToInt32(dr[0].ToString());
                dr.Close();
                con.Close();
                if ((counter == 0))
                {

                    lblconfirm = "This Account is available";

                }
                else
                {
                    lblconfirm = "This Account is Already exist";
                }

            }
            return lblconfirm;
        }


        ////////////populate List//////////////////////////////////////////////////////
        ///

        public List<SelectListItem> PopulateBranchsForAdmins()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (OracleConnection con = new OracleConnection(conString))
            {
                //string query = " select branch_code,branch_name from branchs where branch_sts = '1'";
                string query = "select branch_code,branch_name_en from branchs where branch_status = 'A'";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {

                            //items.Add(new SelectListItem
                            //{
                            //    Text = "-- Select Branch --",
                            //    Value = "0",
                            //});
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["branch_name_en"].ToString(),
                                    Value = sdr["branch_code"].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        public List<SelectListItem> populateadmins()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select * from security_master";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {

                            items.Add(new SelectListItem
                            {
                                Text = "All",
                                Value = "All",
                            });
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["user_name"].ToString(),
                                    Value = sdr["user_log"].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<SelectListItem> PopulateBranchs(string branchcode)
        {
            string query;
            List<SelectListItem> items = new List<SelectListItem>();
            //string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (OracleConnection con = new OracleConnection(conString))
            {
                if (branchcode == "000")
                {
                    query = " select branch_code,branch_name_en from branchs where branch_status = 'A'";
                }
                else
                {
                    query = " select branch_code,branch_name_en from branchs where branch_status = 'A' and BRANCH_CODE ='" + branchcode + "' ";
                }
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {


                            //items.Add(new SelectListItem
                            //{
                            //    Text = "-- Select Branch --",
                            //    Value = "0",
                            //});
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["branch_name_en"].ToString(),
                                    Value = sdr["branch_code"].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        public List<SelectListItem> PopulateBranchsJZ()
        {
            string query;
            List<SelectListItem> items = new List<SelectListItem>();
            //string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (OracleConnection con = new OracleConnection(conString))
            {
                query = "select branch_code, branch_name_en, branch_name_ar from branchs where branch_status = 'A' order by branch_code";
                //query = " select branch_code,branch_name from branchs where branch_sts = '1' order by branch_code";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {

                            items.Add(new SelectListItem
                            {
                                Text = "All Branchs",
                                Value = "000",
                            });
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["branch_name_en"].ToString(),
                                    Value = sdr["branch_code"].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }


        public List<SelectListItem> PopulateBranchs()
        {
            string query;
            List<SelectListItem> items = new List<SelectListItem>();
            //string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (OracleConnection con = new OracleConnection(conString))
            {
                query = "select branch_code, branch_name_en, branch_name_ar from branchs where branch_status = 'A' order by branch_code";
                //query = " select branch_code,branch_name from branchs where branch_sts = '1' order by branch_code";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {

                            items.Add(new SelectListItem
                            {
                                Text = "All Branchs",
                                Value = "000",
                            });
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["branch_name_en"].ToString(),
                                    Value = sdr["branch_code"].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        public List<SelectListItem> PopulateServicess()
        {

            List<SelectListItem> service = new List<SelectListItem>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select service_name,service_code from SERVICES where service_status='A' ", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {

                    service.Add(new SelectListItem
                    {
                        Text = "All Services",
                        Value = "000",
                    });
                    while (dr.Read())
                    {


                        service.Add(new SelectListItem
                        {
                            Text = dr["service_name"].ToString(),
                            Value = dr["service_code"].ToString()

                        });
                    }
                }


            }
            return service;
        }
        public CustomerRegBankinfo GetUserinfoDataLink(string idorname)
        {
            CustomerRegBankinfo usermodel = new CustomerRegBankinfo();
            //char[] chararray = idorname.ToCharArray();
            //if (char.IsDigit(chararray[0]) && idorname.Length == 12)
            //{
            using (OracleConnection con = new OracleConnection(conString))
            {
                //string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_name, SUBSTR(def_acc,6,5) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,5)) as account_type,SUBSTR(def_acc,11,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name from users where user_id = '" + int.Parse(idorname) + "'";
                // string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_name, SUBSTR(def_acc,6,6) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,6)) as account_type,SUBSTR(def_acc,12,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name,SUBSTR(def_acc,19,2) as subno, SUBSTR(def_acc,23,3) as subgl from users where user_log = '" + idorname + "' or user_mobile = '" + idorname + "' ";
                //string query = "select user_name_en , acc_no ,acc_type,acc_curr,acc_branch from users_jsb inner join user_acc_link_jsb on users_jsb.user_log = user_acc_link_jsb.user_id  and user_log = '" + idorname + "'";
                string query = "select user_name_en, user_log, acc_no, acc_type, acc_curr, acc_branch from users_jsb inner join user_acc_link_jsb on users_jsb.user_log = user_acc_link_jsb.user_id and user_log = :idorname";

                OracleCommand cmd = new OracleCommand(query, con);  //SUBSTR(def_acc,12,7)
                cmd.Parameters.Add("idorname", OracleType.VarChar).Value = idorname;
                con.Open();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {

                        usermodel.BranchCode = dataReader["acc_branch"].ToString();  //acc_branch
                        // usermodel.Branch = dataReader["branch_name"].ToString();
                        //usermodel.CurrencyCode = dataReader["currency_code"].ToString();
                        usermodel.Currency = dataReader["acc_curr"].ToString();
                        usermodel.AccountTypecode = dataReader["acc_type"].ToString(); //acc_type
                        //usermodel.AccountType = dataReader["account_type"].ToString();
                        usermodel.AccountNumber = dataReader["acc_no"].ToString();
                        // usermodel.CategoryCode = dataReader["category_id"].ToString();
                        //usermodel.category = dataReader["category_name"].ToString();
                        //usermodel.SUBNO = dataReader["subno"].ToString();
                        // usermodel.SUBGL = dataReader["subgl"].ToString();
                        usermodel.CustomerName = dataReader["user_name_en"].ToString();
                        usermodel.CustomerID = dataReader["user_log"].ToString();  //user_name_en;

                    }
                }
                return usermodel;
            }
            //}
            //else if (char.IsDigit(chararray[0]))
            //{
            //    using (OracleConnection con = new OracleConnection(conString))
            //    {
            //        //string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_name, SUBSTR(def_acc,6,5) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,5)) as account_type,SUBSTR(def_acc,11,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name from users where user_id = '" + int.Parse(idorname) + "'";
            //        string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_name, SUBSTR(def_acc,6,6) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,6)) as account_type,SUBSTR(def_acc,12,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name,SUBSTR(def_acc,19,2) as subno, SUBSTR(def_acc,23,3) as subgl from users where user_id = '" + idorname + "'";
            //        OracleCommand cmd = new OracleCommand(query, con);
            //        con.Open();
            //        using (IDataReader dataReader = cmd.ExecuteReader())
            //        {
            //            while (dataReader.Read())
            //            {
            //                usermodel.BranchCode = dataReader["branch_code"].ToString();
            //                usermodel.Branch = dataReader["branch_name"].ToString();
            //                usermodel.CurrencyCode = dataReader["currency_code"].ToString();
            //                usermodel.Currency = dataReader["currency_name"].ToString();
            //                usermodel.AccountTypecode = dataReader["account_type_code"].ToString();
            //                usermodel.AccountType = dataReader["account_type"].ToString();
            //                usermodel.AccountNumber = dataReader["account_number"].ToString();
            //                usermodel.CategoryCode = dataReader["category_id"].ToString();
            //                usermodel.category = dataReader["category_name"].ToString();
            //                usermodel.SUBNO = dataReader["subno"].ToString();
            //                usermodel.SUBGL = dataReader["subgl"].ToString();
            //                usermodel.CustomerID = idorname;
            //            }
            //        }
            //        return usermodel;
            //    }
            //}
            //else
            //{
            //    using (OracleConnection con = new OracleConnection(conString))
            //    {
            //        //string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_name, SUBSTR(def_acc,6,5) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,5)) as account_type,SUBSTR(def_acc,11,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name from users where user_log = '" + idorname + "'";
            //        string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_name, SUBSTR(def_acc,6,6) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,6)) as account_type,SUBSTR(def_acc,12,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name,SUBSTR(def_acc,19,2) as subno, SUBSTR(def_acc,23,3) as subgl from users where user_log = '" + idorname + "'";
            //        OracleCommand cmd = new OracleCommand(query, con);
            //        con.Open();
            //        using (IDataReader dataReader = cmd.ExecuteReader())
            //        {
            //            while (dataReader.Read())
            //            {
            //                usermodel.BranchCode = dataReader["branch_code"].ToString();
            //                usermodel.Branch = dataReader["branch_name"].ToString();
            //                usermodel.CurrencyCode = dataReader["currency_code"].ToString();
            //                usermodel.Currency = dataReader["currency_name"].ToString();
            //                usermodel.AccountTypecode = dataReader["account_type_code"].ToString();
            //                usermodel.AccountType = dataReader["account_type"].ToString();
            //                usermodel.AccountNumber = dataReader["account_number"].ToString();
            //                usermodel.CategoryCode = dataReader["category_id"].ToString();
            //                usermodel.category = dataReader["category_name"].ToString();
            //                usermodel.SUBNO = dataReader["subno"].ToString();
            //                usermodel.SUBGL = dataReader["subgl"].ToString();
            //                usermodel.CustomerID = idorname;
            //            }
            //        }
            //        return usermodel;
            //    }
            //}
        }

        public List<SelectListItem> PopulateCurrencies()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select curr_code,curr_name_en from currency where CURR_STATUS='A'";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["curr_name_en"].ToString(),
                                Value = sdr["curr_code"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        public List<SelectListItem> PopulateCurrencies(string currency_code)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select curr_code,curr_name_en from currency where curr_status='A' and curr_code = :currency_code";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.Add("currency_code", OracleType.VarChar).Value = currency_code;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["curr_name"].ToString(),
                                Value = sdr["curr_code"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        public List<SelectListItem> PopulateAccountTypes()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select act_type_code,act_name_en from Act_types where act_status = 'A'";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["act_name_en"].ToString(),
                                Value = sdr["act_type_code"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }







        internal List<SelectListItem> PopulateProfiles()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select roleid,name  from TBL_ROLEMASTER where active='A'";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["name"].ToString(),
                                Value = sdr["roleid"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        internal List<SelectListItem> PopulateProfiles(string userid)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select TBL_ROLEMASTER.roleid,TBL_ROLEMASTER.name from TBL_ROLEMASTER inner join users_jsb on TBL_ROLEMASTER.ROLEID = users_jsb.roleid where TBL_ROLEMASTER.active='A' and user_log = :userid";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.Add("userid", OracleType.VarChar).Value = userid;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["name"].ToString(),
                                Value = sdr["roleid"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        internal List<SelectListItem> PopulatecpanelProfiles()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                //string query = "select roleid,name  from cpanel_rolemaster where active='1'";
                string query = "select role_id,role_name  from jsb_roles_master where role_status='A'";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["role_name"].ToString(),
                                Value = sdr["role_id"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        internal List<SelectListItem> PopulatecpanelProfiles(string user_branch)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "";
                if (user_branch == "000")
                {
                    //query = "select roleid,name  from cpanel_rolemaster where active='1'";
                    query = "select role_id,role_name  from jsb_roles_master where role_status='A'";
                }
                else
                {
                    query = "select role_id,role_name  from jsb_roles_master where role_status='A'and role_name <> 'Admin'";
                }

                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["role_name"].ToString(),
                                Value = sdr["role_id"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public Boolean usernameavailabilitycheck(string CustomerUsername)
        {
            Boolean result = true;
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select * from users where user_log = :CustomerUsername";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.Add("CustomerUsername", OracleType.VarChar).Value = CustomerUsername;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            result = false;
                        }
                    }
                    con.Close();
                }
            }

            return result;
        }

        public List<CustomerReportModel> GetCustomersByAdmin(string admin, string fromdate, string todate , int PageNumber)
        {
            OracleCommand cmd;
            OracleDataReader dr;
            string query1;
            string sqlinc = "";

            int offset = PageNumber * 500;
            sqlinc = " OFFSET " + offset + "  ROWS FETCH NEXT 500 ROWS ONLY ";
            List<CustomerReportModel> customers = new List<CustomerReportModel>();
            query1 = "select user_name,user_log,user_email,user_mobile,user_adrs,decode(user_status,'A','Active','U','Authorized','P','Pending','D','Deactive','B','Blocked','DE','Deleted') as status,def_acc,created_by,created_date from users where user_id > 0";

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand();
                cmd.Connection = con;
                if (admin != "All")
                {
                    query1 += " and created_by = :admin";
                    cmd.Parameters.Add("admin", OracleType.VarChar).Value = admin;
                }
                if (fromdate != "All" && fromdate != null)
                {
                    query1 += " and to_date(substr(created_date,0,9),'dd-mon-yy') >= to_date(:fromdate,'mm/dd/yyyy') and to_date(substr(created_date,0,9),'dd-mon-yy') <= to_date(:todate,'mm/dd/yyyy')";
                    cmd.Parameters.Add("fromdate", OracleType.VarChar).Value = fromdate;
                    cmd.Parameters.Add("todate", OracleType.VarChar).Value = todate;
                }
                query1 += " " + sqlinc;
                cmd.CommandText = query1;

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        customers.Add(new CustomerReportModel
                        {
                            CustomerName = dr[0].ToString(),
                            CustomerLog = dr[1].ToString(),
                            Email = dr[2].ToString(),
                            mobile = dr[3].ToString(),
                            address = dr[4].ToString(),
                            CustStatus = dr[5].ToString(),
                            AccountNumber = dr[6].ToString(),
                            created_by = dr[7].ToString(),
                            created_date = dr[8].ToString()
                        });
                    }
                }
            }
            return customers;
        }

        public List<CustomerReportModel> PrintGetCustomersByAdmin(string admin, string fromdate, string todate)
        {
            OracleCommand cmd;
            OracleDataReader dr;
            string query1;

            List<CustomerReportModel> customers = new List<CustomerReportModel>();
            query1 = "select user_name,user_log,user_email,user_mobile,user_adrs,decode(user_status,'A','Active','U','Authorized','P','Pending','D','Deactive','B','Blocked','DE','Deleted') as status,def_acc,created_by,created_date from users where user_id > 0";

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand();
                cmd.Connection = con;
                if (admin != "All")
                {
                    query1 += " and created_by = :admin";
                    cmd.Parameters.Add("admin", OracleType.VarChar).Value = admin;
                }
                if (fromdate != "All" && fromdate != null)
                {
                    query1 += " and to_date(substr(created_date,0,9),'dd-mon-yy') >= to_date(:fromdate,'mm/dd/yyyy') and to_date(substr(created_date,0,9),'dd-mon-yy') <= to_date(:todate,'mm/dd/yyyy')";
                    cmd.Parameters.Add("fromdate", OracleType.VarChar).Value = fromdate;
                    cmd.Parameters.Add("todate", OracleType.VarChar).Value = todate;
                }
                cmd.CommandText = query1;

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        customers.Add(new CustomerReportModel
                        {
                            CustomerName = dr[0].ToString(),
                            CustomerLog = dr[1].ToString(),
                            Email = dr[2].ToString(),
                            mobile = dr[3].ToString(),
                            address = dr[4].ToString(),
                            CustStatus = dr[5].ToString(),
                            AccountNumber = dr[6].ToString(),
                            created_by = dr[7].ToString(),
                            created_date = dr[8].ToString()
                        });
                    }
                }
            }
            return customers;
        }

        public int custreg(string CustomerID, string CustomerName, string account, string userfullaccount, string username, string address, string CustomerPhone, string email, string customerprofile, string customercatgory, string CUSTOMERSERVICE, string created_by)
        {
            int result = -1;
            if (email == null) { email = "N/A"; }

            if (customercatgory == "2")
            {
                username = username + "O";
            }
            else if (customercatgory == "3")
            {
                username = username + "A";
            }

            Random random = new Random();

            if (usernameavailabilitycheck(username))
            {
                using (OracleConnection con = new OracleConnection(conString))
                {
                    //String re = CreatePassword(8);  generating 8 random characters
                    String re = random.Next(10000000, 99999999).ToString(); // generating 8 random numbers

                    String enc_pwd = Encrypt(re);

                    //                string query = "INSERT INTO users (USER_ID,USER_NAME,USER_LOG,USER_PWD,USER_EMAIL,USER_MOBILE,USER_FAX,USER_ADRS,USER_STATUS,DEF_ACC,LAST_LOGIN,LAST_LOG_IP,FAILD_LOGINS,USER_CUSTID,FIRST_LOGIN,CATOGRY,USER_PAS,USER_TRANSFER,ROLEID,ACCOUNT,ACTIVE)" +
                    //"VALUES((select max( to_number(user_id))+1 from users),'" + CustomerName + "','" + username + "','" + enc_pwd + "','" + email + "','" + CustomerPhone + "','" + CustomerPhone + "','al-khaleejbank','P','" + account + "',sysdate,'127.0.0.1',0,'"+CustomerID+"','T','"+customercatgory+"','" + re + "','True','" + customerprofile + "','" + account + "','1')";

                    //                OracleCommand cmd = new OracleCommand(query, con);

                    //                con.Open();

                    //                result = cmd.ExecuteNonQuery();
                    OracleCommand cmd = new OracleCommand();
                    cmd.CommandText = "insertnewcustomer";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Connection = con;
                    con.Open();

                    cmd.Parameters.Add("CustomerName", OracleType.VarChar).Value = CustomerName;
                    cmd.Parameters.Add("username", OracleType.VarChar).Value = username;
                    cmd.Parameters.Add("enc_pwd", OracleType.VarChar).Value = enc_pwd;
                    cmd.Parameters.Add("email", OracleType.VarChar).Value = email;
                    cmd.Parameters.Add("CustomerPhone", OracleType.VarChar).Value = CustomerPhone;
                    cmd.Parameters.Add("useraccount", OracleType.VarChar).Value = account;
                    cmd.Parameters.Add("userfullaccount", OracleType.VarChar).Value = userfullaccount;
                    cmd.Parameters.Add("CustomerID", OracleType.VarChar).Value = CustomerID;
                    //cmd.Parameters.Add("CustomerPhone", OracleType.VarChar).Value = CustomerPhone;
                    //cmd.Parameters.Add("useraccount", OracleType.VarChar).Value = account;
                    cmd.Parameters.Add("customercatgory", OracleType.VarChar).Value = customercatgory;
                    cmd.Parameters.Add("re", OracleType.VarChar).Value = re;
                    cmd.Parameters.Add("customerprofile", OracleType.VarChar).Value = customerprofile;
                    cmd.Parameters.Add("CUSTOMERSERVICE", OracleType.VarChar).Value = CUSTOMERSERVICE;
                    cmd.Parameters.Add("Customeraddress", OracleType.VarChar).Value = address;
                    cmd.Parameters.Add("createdby", OracleType.VarChar).Value = created_by;
                    cmd.Parameters.Add("res", OracleType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("errcode", OracleType.VarChar, 4000).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("errmsg", OracleType.VarChar, 4000).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    String res = cmd.Parameters["res"].Value.ToString();
                    String errormsg = cmd.Parameters["errmsg"].Value.ToString();
                    String errorcode = cmd.Parameters["errcode"].Value.ToString();
                    result = Int32.Parse(res);
                }
                return result;
            }
            else
            {
                result = 2;
                return result;
            }
        }

        public List<CustomerAuthorization> PendingCustomer(String bracode)
        {
            OracleCommand cmd;
            OracleDataReader dr;

            String userid, username, useract;
            String query1, result;
            List<CustomerAuthorization> customer = new List<CustomerAuthorization>();

            if (bracode != "000")
            {
                query1 = "select users.user_id,users.user_name,SUBSTR(def_acc,3,11) from users inner join security_master on created_by = security_master.user_log where user_status = 'P' and security_master.user_branch = :bracode";
            }
            else
            {
                query1 = "select users.user_id,users.user_name,SUBSTR(def_acc,3,11) from users inner join security_master on created_by = security_master.user_log where user_status = 'P'";
            }


            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);
                if (bracode != "000")
                {
                    cmd.Parameters.Add("bracode", OracleType.VarChar).Value = bracode;
                }

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        userid = dr[0].ToString();
                        username = dr[1].ToString();
                        useract = dr[2].ToString();


                        customer.Add(new CustomerAuthorization
                        {
                            CustomerID = userid,
                            Customername = username,
                            Customeraccount = useract
                        });
                    }
                }


            }


            return customer;
        }

        public int insertadminslog(string userid, string username, string branch, string userrole, string userstatus, string action, string actiononuser, string timestamp)
        {
            int result = -1;

            if (usernameavailabilitycheck(username))
            {
                using (OracleConnection con = new OracleConnection(conString))
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.CommandText = "INSERTADMINSLOG";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();

                    cmd.Parameters.Add("user_id", OracleType.VarChar).Value = userid;
                    cmd.Parameters.Add("username", OracleType.VarChar).Value = username;
                    cmd.Parameters.Add("user_role", OracleType.VarChar).Value = userrole;
                    cmd.Parameters.Add("user_status", OracleType.VarChar).Value = userstatus;
                    cmd.Parameters.Add("action", OracleType.VarChar).Value = action;
                    cmd.Parameters.Add("action_on_user", OracleType.VarChar).Value = actiononuser;
                    cmd.Parameters.Add("timedate", OracleType.VarChar).Value = timestamp;
                    cmd.Parameters.Add("user_branch", OracleType.VarChar).Value = branch;
                    OracleParameter p1 = new OracleParameter("status", OracleType.VarChar, 2000);
                    p1.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(p1);

                    result = cmd.ExecuteNonQuery();
                }
                return result;
            }
            else
            {
                result = 2;
                return result;
            }
        }


       



        public List<CustomerAuthorizationinfo> CustomerAuthorizationinfo(String userid)
        {
            OracleCommand cmd;
            OracleDataReader dr;

            String username, useract;
            String query1, result;
            List<CustomerAuthorizationinfo> customer = new List<CustomerAuthorizationinfo>();
            OracleDataReader dr3;
            OracleCommand cmd3;
            string sqstr;
            string msg = "";
            string br, Sessioncurr = "";
            String acc = "";
            String acc_type = "";
            String acc_no;
            String curr;
            String curr_name = "";
            String lang;
            String brname = "";
            String acctype = "";
            String roleid = "", profilename = "";
            query1 = "select * from users_jsb where user_id = :userid";
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd3 = new OracleCommand(query1, con);
                cmd3.Parameters.Add("userid", OracleType.VarChar).Value = userid;

                con.Open();


                dr3 = cmd3.ExecuteReader();
                if (dr3.Read())
                {
                    // 'lb_cust_name.Text = dr3(1)
                    OracleDataReader dr2;
                    OracleDataReader dr4;
                    OracleDataReader dr5;
                    OracleDataReader dr6;
                    OracleCommand cmd2;
                    OracleCommand cmd4;
                    OracleCommand cmd5;
                    OracleCommand cmd6;


                    acc = dr3[9].ToString();
                    roleid = dr3[18].ToString();
                    br = acc.Substring(2, 3);
                    acc_type = acc.Substring(5, 5);
                    Sessioncurr = acc.Substring(10, 3);
                    acc_no = acc.Substring(13);

                    cmd4 = new OracleCommand("select BRANCH_NAME from BRANCHS where BRANCH_CODE_NO = :br", con);
                    cmd4.Parameters.Add("br", OracleType.VarChar).Value = br;
                    dr4 = cmd4.ExecuteReader();
                    if (dr4.Read())
                    {
                        brname = dr4[0].ToString();

                    }

                    dr4.Close();
                    //cmd5 = new OracleCommand(("select act_name from act_types where act_type_code ='" + (acc_type + "'")), con);
                    //dr5 = cmd5.ExecuteReader();
                    //if (dr5.Read())
                    //{
                    //    acctype = dr5[0].ToString();

                    //}

                    //dr5.Close();
                    cmd5 = new OracleCommand("select act_name from act_types where act_type_code = :acc_type", con);
                    cmd5.Parameters.Add("acc_type", OracleType.VarChar).Value = acc_type;
                    dr5 = cmd5.ExecuteReader();
                    if (dr5.HasRows)
                    {
                        dr5.Read();
                        acctype = dr5[0].ToString();

                    }
                    else
                    {
                        cmd5 = new OracleCommand("select act_name from act_types where act_type_code = :acc_type", con);
                        cmd5.Parameters.Add("acc_type", OracleType.VarChar).Value = acc_type;
                        dr5 = cmd5.ExecuteReader();
                        if (dr5.HasRows)
                        {
                            dr5.Read();
                            acctype = dr5[0].ToString();

                        }
                        else
                            acctype = "Account Type Not Found";

                    }

                    dr5.Close();
                    cmd2 = new OracleCommand("select name from tbl_rolemaster where roleid = :roleid", con);
                    cmd2.Parameters.Add("roleid", OracleType.VarChar).Value = roleid;
                    dr2 = cmd2.ExecuteReader();
                    if (dr2.Read())
                    {
                        profilename = dr2[0].ToString();

                    }


                    dr2.Close();
                    cmd6 = new OracleCommand("select CURR_NAME from CURRENCY where CURR_STS='1' and CURR_CODE = :Sessioncurr", con);
                    cmd6.Parameters.Add("Sessioncurr", OracleType.VarChar).Value = Sessioncurr;
                    dr6 = cmd6.ExecuteReader();
                    if (dr6.Read())
                    {
                        curr_name = dr6[0].ToString();

                    }

                    dr6.Close();
                }
                customer.Add(new CustomerAuthorizationinfo
                {
                    userid = dr3[0].ToString(),
                    //Branch = brname,
                    //AccountType = acctype,
                    Customername = dr3[1].ToString(),
                    //Currency = curr_name,
                    Customeraccount = acc.Substring(2, 11),
                    UserName = dr3[2].ToString(),
                    Address = dr3[7].ToString(),
                    CustomerPhone = dr3[5].ToString(),
                    Email = dr3[4].ToString(),
                    //Profile = profilename,
                });

                dr3.Close();
            }
            return customer;

        }

        public int Insertaccounttype(string account_type_code, string account_type, string account_type_arabic, string account_type_no, string account_type_status)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "insertaccounttype";

                cmd.Parameters.Add("account_type_code", OracleType.VarChar).Value = account_type_code;
                cmd.Parameters.Add("account_type", OracleType.VarChar).Value = account_type;
                cmd.Parameters.Add("account_type_arabic", OracleType.VarChar).Value = account_type_arabic;
                cmd.Parameters.Add("account_type_no", OracleType.VarChar).Value = account_type_no;
                cmd.Parameters.Add("account_type_status", OracleType.VarChar).Value = account_type_status;
                OracleParameter p3 = new OracleParameter("res", OracleType.Int32);
                p3.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p3);
                OracleParameter p4 = new OracleParameter("errcode", OracleType.VarChar, 2000);
                p4.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p4);
                OracleParameter p5 = new OracleParameter("errmsg", OracleType.VarChar, 2000);
                p5.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p5);

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        public AccountTypeModel getaccounttypedetails(string account_type_code)
        {
            AccountTypeModel account_type = new AccountTypeModel();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select act_type_code,act_name,act_name_ar,act_type_id from act_types where act_type_code = :account_type_code", con);
                cmd.Parameters.Add("account_type_code", OracleType.VarChar).Value = account_type_code;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    account_type.account_type_code = dr["act_type_code"].ToString();
                    account_type.account_type = dr["act_name"].ToString();
                    account_type.account_type_arabic = dr["act_name_ar"].ToString();
                    account_type.account_type_no = dr["act_type_id"].ToString();
                }
                else
                {
                    dr.Close();
                }
                dr.Close();
                con.Close();
            }
            return account_type;
        }

        public BranchModel getbranchdetails(string branch_code)
        {
            BranchModel branch = new BranchModel();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select branch_code,branch_name,branch_sts,branch_code_no,branch_db_link,branch_name_ar from branchs where branch_code = :branch_code", con);
                cmd.Parameters.Add("branch_code", OracleType.VarChar).Value = branch_code;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    branch.branch_code = dr["branch_code"].ToString();
                    branch.branch_name = dr["branch_name"].ToString();
                    branch.branch_status = dr["branch_sts"].ToString();
                    branch.branch_code_no = dr["branch_code_no"].ToString();
                    branch.branch_db_link = dr["branch_db_link"].ToString();
                    branch.branch_name_arabic = dr["branch_name_ar"].ToString();
                }
                else
                {
                    dr.Close();
                }
                dr.Close();
                con.Close();
            }
            return branch;
        }

        public int deleteAccountType(string account_type_code)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("delete from act_types where act_type_code = :account_type_code", con);
                cmd.Parameters.Add("account_type_code", OracleType.VarChar).Value = account_type_code;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        public int UpdateAccountType(AccountTypeModel model)
        {

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update act_types set act_name = :account_type, act_name_ar = :account_type_arabic, act_type_id = :account_type_no where act_type_code = :account_type_code", con);
                cmd.Parameters.Add("account_type", OracleType.VarChar).Value = model.account_type;
                cmd.Parameters.Add("account_type_arabic", OracleType.VarChar).Value = model.account_type_arabic;
                cmd.Parameters.Add("account_type_no", OracleType.VarChar).Value = model.account_type_no;
                cmd.Parameters.Add("account_type_code", OracleType.VarChar).Value = model.account_type_code;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        public int Insertcurrency(string currency_code, string currency_name, string currency_summary, string currency_status)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "insertcurrency";

                cmd.Parameters.Add("currency_code", OracleType.VarChar).Value = currency_code;
                cmd.Parameters.Add("currency_name", OracleType.VarChar).Value = currency_name;
                cmd.Parameters.Add("currency_summary", OracleType.VarChar).Value = currency_summary;
                cmd.Parameters.Add("curreny_status", OracleType.VarChar).Value = currency_status;
                OracleParameter p3 = new OracleParameter("res", OracleType.Int32);
                p3.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p3);
                OracleParameter p4 = new OracleParameter("errcode", OracleType.VarChar, 2000);
                p4.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p4);
                OracleParameter p5 = new OracleParameter("errmsg", OracleType.VarChar, 2000);
                p5.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p5);

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        public CurrencyModel getcurrencydetails(string currency_code)
        {
            CurrencyModel currency = new CurrencyModel();

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select curr_code,curr_name,curr_sumry,curr_sts from currency where curr_code = :currency_code", con);
                cmd.Parameters.Add("currency_code", OracleType.VarChar).Value = currency_code;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    currency.currency_code = dr["curr_code"].ToString();
                    currency.currency_name = dr["curr_name"].ToString();
                    currency.currency_summary = dr["curr_sumry"].ToString();
                    currency.currency_status = dr["curr_sts"].ToString();
                }
                else
                {
                    dr.Close();
                }
                dr.Close();
                con.Close();
            }
            return currency;
        }

        public int deletecurrency(string currency_code)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("delete from currency where curr_code = :currency_code", con);
                cmd.Parameters.Add("currency_code", OracleType.VarChar).Value = currency_code;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        public int updatecurrency(CurrencyModel model)
        {

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("Update currency set curr_name = :currency_name, curr_sumry = :currency_summary, curr_sts = :currency_status where curr_code = :currency_code", con);
                cmd.Parameters.Add("currency_name", OracleType.VarChar).Value = model.currency_name;
                cmd.Parameters.Add("currency_summary", OracleType.VarChar).Value = model.currency_summary;
                cmd.Parameters.Add("currency_status", OracleType.VarChar).Value = model.currency_status;
                cmd.Parameters.Add("currency_code", OracleType.VarChar).Value = model.currency_code;
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        public int updatecustomer(String userid, String status)
        {
            OracleCommand cmd;
            int result = -1;


            String query1;
            query1 = "update users set USER_STATUS='" + status + "' where user_id='" + userid + "'";
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();
                result = cmd.ExecuteNonQuery();
            }
            return result;
        }
        public int updatecustomerusingact(String account, String status)
        {
            OracleCommand cmd;
            int result = -1;


            String query1;
            query1 = "update USERS set USER_STATUS ='" + status + "',FAILD_LOGINS=0 where DEF_ACC ='" + account + "'";
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();
                result = cmd.ExecuteNonQuery();
            }
            return result;
        }

        public List<Custreport> getbranchcustomers(string branchcode)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query;

                //if (branchcode == "000")
                //{
                    query = "select USER_ID,USER_NAME_EN,USER_LOG,USER_EMAIL,USER_MOBILE,USER_ADDRESS,  decode(users_jsb.USER_STATUS , 'A','Active','B','Blocked','D','DeActive' , 'P', 'Pendding','U' , 'Authorized','R','Rejected','DE','Deleted','S','Stopped') as USER_STATUS ,CREATED_BY ,DEF_ACC from users_jsb";
                //}
                //else
                //{

                //    query = "select USER_ID,USER_NAME_EN,USER_LOG,USER_EMAIL,USER_MOBILE,USER_ADDRESS,   decode(users_jsb.USER_STATUS , 'A','Active','B','Blocked','D','DeActive' , 'P', 'Pendding','U' , 'Authorized','R','Rejected','DE','Deleted','S','Stopped') as USER_STATUS ,CREATED_BY ,DEF_ACC from users_jsb where SUBSTR(DEF_ACC,3,3) = '" + branchcode + "'";
                //}
                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();

                List<Custreport> customers = new List<Custreport>();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {

                    while (dataReader.Read())
                    {
                        Custreport obj = new Custreport();
                        obj.CustomerID = dataReader["USER_ID"].ToString();
                        obj.customerfullname = dataReader["USER_NAME_EN"].ToString();
                        obj.CustomerName = dataReader["USER_LOG"].ToString();
                        obj.customeremail = dataReader["USER_EMAIL"].ToString();
                        obj.phonenumber = dataReader["USER_MOBILE"].ToString();
                        obj.address = dataReader["USER_ADDRESS"].ToString();
                        obj.CustStatus = dataReader["USER_STATUS"].ToString();
                        obj.AccountNumber = dataReader["DEF_ACC"].ToString();
                        //obj.lastlogin = dataReader["LAST_LOGIN"].ToString();
                        //obj.lastip = dataReader["LAST_LOG_IP"].ToString();
                        //obj.faildlogincount = dataReader["FAILD_LOGINS"].ToString();
                        //obj.category = dataReader["CATOGRY"].ToString();
                        obj.createdby = dataReader["CREATED_BY"].ToString();
                        customers.Add(obj);
                    }
                    return customers;
                }
            }
        }


        public Loginmodelresult checkuserlogin(String usrname, String password, String UserHostAddress)
        {
            Loginmodelresult model = new Loginmodelresult();
            model.Login = false;

            using (OracleConnection con = new OracleConnection(conString))
            {
                // WAPT01-01: Parameterized login query — no string concatenation.
                // WAPT11: fetch the stored password hash and verify in code (salted
                // PBKDF2 cannot be matched by a SQL equality check).
                OracleCommand cmd = new OracleCommand(
                    "SELECT user_id, user_name, user_branch, user_last_login, roleid, user_status, user_pwd " +
                    "FROM jsb_security_master " +
                    "WHERE user_LOG = :usrname AND user_status = 'A'", con);
                cmd.Parameters.Add("usrname", OracleType.VarChar).Value = usrname;

                try
                {
                    con.Open();
                    OracleDataReader dr = cmd.ExecuteReader();

                    bool authenticated = false;

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string storedPwd = dr["user_pwd"] == null ? null : dr["user_pwd"].ToString();

                            // WAPT11: verify against the stored hash; legacy AES values
                            // are accepted once and flagged for re-hashing.
                            bool needsUpgrade;
                            if (!AljazeeraCPanel.Security.PasswordHasher.Verify(password, storedPwd, out needsUpgrade))
                            {
                                // Password mismatch for this user — treat as failed login.
                                break;
                            }

                            authenticated = true;

                            model.UserId = dr[0].ToString();
                            model.user_name = dr[1].ToString();
                            model.user_branch = dr[2].ToString();
                            model.user_last_login = dr[3].ToString();
                            model.user_roleid = dr[4].ToString();
                            model.status = dr[5].ToString();
                            model.user_log = usrname;
                            model.Login = true;

                            // WAPT11: transparently upgrade a legacy-encrypted password to PBKDF2.
                            if (needsUpgrade)
                            {
                                try
                                {
                                    OracleCommand up = new OracleCommand(
                                        "UPDATE jsb_security_master SET user_pwd = :pwd WHERE user_id = :userId", con);
                                    up.Parameters.Add("pwd", OracleType.VarChar).Value =
                                        AljazeeraCPanel.Security.PasswordHasher.Hash(password);
                                    up.Parameters.Add("userId", OracleType.VarChar).Value = model.UserId;
                                    up.ExecuteNonQuery();
                                }
                                catch { /* upgrade is best-effort; never block a valid login */ }
                            }

                            // WAPT01-01: Parameterized success audit log insert
                            OracleCommand cmd2 = new OracleCommand(
                                "INSERT INTO Users_login VALUES (:ip, :logindate, :uname, '-', 'S')", con);
                            cmd2.Parameters.Add("ip", OracleType.VarChar).Value = UserHostAddress;
                            cmd2.Parameters.Add("logindate", OracleType.VarChar).Value = DateTime.Today.ToString();
                            cmd2.Parameters.Add("uname", OracleType.VarChar).Value = usrname;
                            cmd2.ExecuteNonQuery();

                            if (model.user_last_login == "T")
                            {
                                // WAPT01-01: Parameterized first-login update
                                OracleCommand cmd3 = new OracleCommand(
                                    "UPDATE jsb_SECURITY_MASTER SET user_last_login = 'F' WHERE user_id = :userId", con);
                                cmd3.Parameters.Add("userId", OracleType.VarChar).Value = model.UserId;
                                cmd3.ExecuteNonQuery();
                                model.lblconfirm = "change_pass";
                            }
                            else
                            {
                                model.lblconfirm = "home";
                            }
                        }
                    }

                    if (!authenticated)
                    {
                        // WAPT01-01: Parameterized failed login audit log insert
                        OracleCommand cmd2 = new OracleCommand(
                            "INSERT INTO Users_login VALUES (:ip, :logindate, :uname, '-', 'F')", con);
                        cmd2.Parameters.Add("ip", OracleType.VarChar).Value = UserHostAddress;
                        cmd2.Parameters.Add("logindate", OracleType.VarChar).Value = DateTime.Today.ToString();
                        cmd2.Parameters.Add("uname", OracleType.VarChar).Value = usrname;
                        cmd2.ExecuteNonQuery();
                        model.lblconfirm = "Wrong input username or password";
                    }
                }
                catch (Exception ex)
                {
                    model.lblconfirm = "System Error";
                }
            }
            return model;
        }


        public List<Limit> GetCurrentLimitss()
        {
            Limit result = new Limit();
            List<Limit> limited = new List<Limit>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                //OracleCommand cmd = new OracleCommand("select * from limits where roleid = 0", con);
                //OracleCommand cmd = new OracleCommand("select * from limits ", con);
                OracleCommand cmd = new OracleCommand("select LIMIT_MODEL_AMOUNT_PER_TRAN,LIMIT_MODEL_AMOUNT_PER_DAY,LIMIT_MODEL_NO_PER_DAY,decode (LIMIT_MODEL_NAME ,'A2C Limit' , 'Other Bank Transfer' ,'NEC Limit' , 'SDEC Limit' , LIMIT_MODEL_NAME) as LIMIT_MODEL_NAME  from jsb_limit_models", con);

                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                //OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        limited.Add(new Limit
                        {
                            //Transaction_per_day = int.Parse(dr["TRANS_COUNT"].ToString()),
                            //Transaction_amount = int.Parse(dr["TRANS_AMOUNT"].ToString()),
                            //Transactions_accumulation = int.Parse(dr["TRANS_ACCUMULATIVE"].ToString()),
                            //Fees = int.Parse(dr["fees"].ToString()),
                            //Tax = int.Parse(dr["tax"].ToString()),
                            //service_name = dr["service_name"].ToString(),
                            //serviceid = int.Parse(dr["serviceid"].ToString()),
                            //flag = int.Parse(dr["flag"].ToString())
                            Transaction_per_day = int.Parse(dr["LIMIT_MODEL_NO_PER_DAY"].ToString()),
                            Transaction_amount = double.Parse(dr["LIMIT_MODEL_AMOUNT_PER_TRAN"].ToString()),
                            Transactions_accumulation = double.Parse(dr["LIMIT_MODEL_AMOUNT_PER_DAY"].ToString()),
                            //Fees = int.Parse(dr["fees"].ToString()),
                            //Tax = int.Parse(dr["tax"].ToString()),
                            service_name = dr["LIMIT_MODEL_NAME"].ToString(),
                            //serviceid = int.Parse(dr["serviceid"].ToString()),
                            //flag = int.Parse(dr["flag"].ToString())
                        });
                    }
                }
                else
                {
                    dr.Close();
                }
                dr.Close();
                con.Close();
            }
            return limited;
        }

        public Limit GetCurrentLimits(string serviceid)
        {
            Limit result = new Limit();

            using (OracleConnection con = new OracleConnection(conString))
            {
                //OracleCommand cmd = new OracleCommand("select * from limits where roleid = 0", con);
                //OracleCommand cmd = new OracleCommand("select * from limits where serviceid = " + serviceid + " ", con);
                OracleCommand cmd = new OracleCommand("select  LIMIT_MODEL_AMOUNT_PER_TRAN,LIMIT_MODEL_AMOUNT_PER_DAY,LIMIT_MODEL_NO_PER_DAY,decode (LIMIT_MODEL_NAME ,'A2C Limit' , 'Other Bank Transfer' ,'NEC Limit' , 'SDEC Limit' , LIMIT_MODEL_NAME) as LIMIT_MODEL_NAME  from jsb_limit_models  where LIMIT_MODEL_NAME = '" + serviceid + "' ", con);

                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    //result.Transaction_per_day = int.Parse(dr["TRANS_COUNT"].ToString());
                    //result.Transaction_amount = int.Parse(dr["TRANS_AMOUNT"].ToString());
                    //result.Transactions_accumulation = int.Parse(dr["TRANS_ACCUMULATIVE"].ToString());
                    //result.Fees = int.Parse(dr["fees"].ToString());
                    //result.Tax = int.Parse(dr["tax"].ToString());
                    //result.service_name = dr["service_name"].ToString();
                    //result.serviceid = int.Parse(dr["serviceid"].ToString());
                    //result.flag = int.Parse(dr["flag"].ToString());

                    result.Transaction_per_day = int.Parse(dr["LIMIT_MODEL_NO_PER_DAY"].ToString());
                    result.Transaction_amount = int.Parse(dr["LIMIT_MODEL_AMOUNT_PER_TRAN"].ToString());
                    result.Transactions_accumulation = int.Parse(dr["LIMIT_MODEL_AMOUNT_PER_DAY"].ToString());
                    //result.Fees = int.Parse(dr["fees"].ToString());
                    //result.Tax = int.Parse(dr["tax"].ToString());
                    result.service_name = dr["LIMIT_MODEL_NAME"].ToString();
                    //result.serviceid = int.Parse(dr["serviceid"].ToString());
                    //result.flag = int.Parse(dr["flag"].ToString());
                }
                else
                {
                    dr.Close();
                }
                dr.Close();
                con.Close();
            }
            return result;
        }

        public int updatelimits(Limit newlimits)
        {

            using (OracleConnection con = new OracleConnection(conString))
            {
                // OracleCommand cmd = new OracleCommand("update limits set trans_count = '" + newlimits.Transaction_per_day + "', trans_amount = '" + newlimits.Transaction_amount + "', trans_accumulative = '" + newlimits.Transactions_accumulation + "' , service_name = '" + newlimits.service_name + "' , fees = '" + newlimits.Fees + "' , tax = '" + newlimits.Tax + "' , flag = '" + newlimits.flag + "' where serviceid = " + newlimits.serviceid + " ", con);
                //OracleCommand cmd = new OracleCommand("update limits set trans_count = '" + newlimits.Transaction_per_day + "', trans_amount = '" + newlimits.Transaction_amount + "', trans_accumulative = '" + newlimits.Transactions_accumulation + "'  , fees = '" + newlimits.Fees + "' , tax = '" + newlimits.Tax + "' , flag = '" + newlimits.flag + "'  where   service_name = '" + newlimits.service_name + "' ", con);
                OracleCommand cmd = new OracleCommand("update jsb_limit_models set LIMIT_MODEL_NO_PER_DAY = :Transaction_per_day, LIMIT_MODEL_AMOUNT_PER_TRAN = :Transaction_amount, LIMIT_MODEL_AMOUNT_PER_DAY = :Transactions_accumulation where LIMIT_MODEL_NAME = :service_name", con);
                cmd.Parameters.Add("Transaction_per_day", OracleType.VarChar).Value = newlimits.Transaction_per_day.ToString();
                cmd.Parameters.Add("Transaction_amount", OracleType.VarChar).Value = newlimits.Transaction_amount.ToString();
                cmd.Parameters.Add("Transactions_accumulation", OracleType.VarChar).Value = newlimits.Transactions_accumulation.ToString();
                cmd.Parameters.Add("service_name", OracleType.VarChar).Value = newlimits.service_name;

                if (con.State == ConnectionState.Closed)
                { con.Open(); }
                return cmd.ExecuteNonQuery();
            }
        }
        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }


        protected string Encrypt(string clearText)
        {
            //string EncryptionKey = "IBAZ2TWTQS77898";
            //byte[] cleFCBytes = Encoding.Unicode.GetBytes(clearText);
            //using (Aes encryptor = Aes.Create())
            //{
            //    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            //    encryptor.Key = pdb.GetBytes(32);
            //    encryptor.IV = pdb.GetBytes(16);
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            //        {
            //            cs.Write(cleFCBytes, 0, cleFCBytes.Length);
            //            cs.Close();
            //        }
            //        clearText = Convert.ToBase64String(ms.ToArray());
            //    }
            //}
            CryptLib _crypt = new CryptLib();

            String key = "b16920894899c7780b5fc7161560a412";//CryptLib.SHA256("my secret key", 32); //32 bytes = 256 bit

            String iv = "e77886746a9b416d";
            //String iv = CryptLib.GenerateRandomIV(16); //16 bytes = 128 bits
            //string key = CryptLib.getHashSha256("my secret key", 31); //32 bytes = 256 bits
            String cypherText = _crypt.encrypt(clearText, key, iv);

            //Console.WriteLine("Plain text =" + _crypt.decrypt(cypherText, key, iv));
            return cypherText;
        }

        protected string Decrypt(string cipherText)
        {
            string EncryptionKey = "IBAZ2TWTQS77898";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public String changepass(String usrname, String oldpass, String newpass)
        {
            String lblconfirm = "System Error";

            using (OracleConnection con = new OracleConnection(conString))
            {
                try
                {
                    con.Open();

                    // WAPT01 + WAPT11: parameterized lookup, then verify the old password
                    // against the stored hash in code (no SQL equality on the secret).
                    OracleCommand cmd = new OracleCommand(
                        "SELECT user_pwd FROM jsb_security_master WHERE user_LOG = :usrname", con);
                    cmd.Parameters.Add("usrname", OracleType.VarChar).Value = usrname;

                    string storedPwd = null;
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                            storedPwd = dr["user_pwd"] == null ? null : dr["user_pwd"].ToString();
                    }

                    bool needsUpgrade;
                    if (storedPwd != null &&
                        AljazeeraCPanel.Security.PasswordHasher.Verify(oldpass, storedPwd, out needsUpgrade))
                    {
                        // WAPT11: store the new password as a one-way PBKDF2 hash (parameterized).
                        OracleCommand cmd2 = new OracleCommand(
                            "UPDATE jsb_security_master SET user_pwd = :pwd WHERE user_log = :usrname", con);
                        cmd2.Parameters.Add("pwd", OracleType.VarChar).Value =
                            AljazeeraCPanel.Security.PasswordHasher.Hash(newpass);
                        cmd2.Parameters.Add("usrname", OracleType.VarChar).Value = usrname;
                        cmd2.ExecuteNonQuery();
                        lblconfirm = "Your Password was Changed Successfully";
                    }
                    else
                    {
                        lblconfirm = "Your Password was Not Changed successfully";
                    }
                }
                catch (Exception ex)
                {
                    lblconfirm = "System Error";
                }
            }
            return lblconfirm;
        }



       


        public List<addaccount> Populatecustacts()
        {
            int i = 0; ;
            List<addaccount> items = new List<addaccount>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select  acc_no,branch_name,act_name,curr_name from USER_ACC_LINK acc,branchs br ,CURRENCY cur ,act_types cty" +
                    " where cur.CURR_STS='1' and  substr(acc.acc_no,3,3)= br.branch_code and substr(acc.acc_no,6,5)=cty.ACT_TYPE_CODE" +
                    " and substr(acc.acc_no,11,3)=cur.curr_code ";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new addaccount
                            {
                                AccountID = i + 1,
                                AccountNumber = sdr["acc_no"].ToString().Substring(12, 7),
                                AccountNumbercomplete = sdr["acc_no"].ToString(),
                                Branch = sdr["branch_name"].ToString(),
                                AccountType = sdr["act_name"].ToString(),
                                Currency = sdr["curr_name"].ToString(),
                                IsSelected = false,
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        public String addnewacount(String act, String account, String category)
        {
            String lblconfirm = "System Error", user_id = null;
            bool FLAG;
            OracleCommand cmd;
            OracleDataReader dr;
            using (OracleConnection con = new OracleConnection(conString))
            {
                try
                {
                    cmd = new OracleCommand("select acc_no from user_acc_link  where acc_no='" + account + "'", con);
                    OracleCommand cmd2;
                    OracleCommand cmd_acc_lnk;
                    con.Open();
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        FLAG = false;
                        lblconfirm = "These Account Already exist";
                        con.Close();
                        return lblconfirm;
                    }
                    else
                    {
                        FLAG = true;
                    }

                    if (FLAG == true)
                    {

                        string query = "select user_id,user_name from users where DEF_ACC='" + act + "'and CATOGRY='" + category + "'";

                        OracleCommand cmd3 = new OracleCommand(query, con);
                        OracleDataReader drr = cmd3.ExecuteReader();
                        if (drr.Read())
                        {
                            user_id = drr[0].ToString();
                        }
                        cmd = new OracleCommand("select count(*) from user_acc_link where acc_no='" + account + "' and user_id='" + user_id + "'", con);
                        dr = cmd.ExecuteReader();
                        dr.Read();
                        int counter;
                        counter = Convert.ToInt32(dr[0].ToString());
                        dr.Close();
                        cmd.Dispose();
                        if (counter == 0)
                        {
                            String dp_branch, dp_acc_tybe, dp_acc_curr;
                            dp_acc_tybe = account.Substring(5, 5);
                            dp_branch = account.Substring(2, 3);
                            dp_acc_curr = account.Substring(10, 3);
                            String sql2 = "select  nvl(max (acc_id),0) from user_acc_link where user_id=" + user_id;
                            cmd2 = new OracleCommand(sql2, con);
                            dr = cmd2.ExecuteReader();
                            dr.Read();
                            int ACC_ID;
                            ACC_ID = Convert.ToInt32(dr[0].ToString());
                            dr.Close();
                            cmd2.Dispose();
                            ACC_ID = ACC_ID + 1;
                            cmd_acc_lnk = new OracleCommand("INSERT INTO user_acc_link (BRANCH_CODE,ACT_TYPE,USER_ID,ACC_NO,ACC_STS,ACC_CURR,ACC_LANG,ACC_STATUS,ACC_ID,CATOGRY) values ('"
                                             + dp_branch + "','" + dp_acc_tybe + "','" + user_id + "','" + account + "','P','" + dp_acc_curr + "','AR','P','" + ACC_ID + "',  '" + category + "')", con);
                            cmd_acc_lnk.ExecuteNonQuery();
                            lblconfirm = "Account Added Successfully";
                        }
                        else
                        {
                            lblconfirm = "These Account Already exist";
                        }
                        con.Close();
                    }


                }
                catch (Exception ex)
                {
                    lblconfirm = "System Error : " + ex.Message;
                }
            }
            return lblconfirm;
        }

        public String addnewacountforFrist(String userlog, String Account_No, String Account_Type_Code, String Branch_Code, String Currency_Code, String IBAN, Boolean acc_prim)
        {
            String lblconfirm = "System Error";
            string acc_app_type = "";

            using (OracleConnection con = new OracleConnection(conString))
            {
                try
                {
                    if (Account_Type_Code.Equals("SAV"))
                    {
                        acc_app_type = "SV";
                    }
                    else
                    {
                        acc_app_type = "CK";
                    }

                    String sql2 = "select nvl(max(acc_id),0) from user_acc_link_jsb";
                    OracleCommand cmd2 = new OracleCommand(sql2, con);
                    con.Open();
                    OracleDataReader dr = cmd2.ExecuteReader();
                    dr.Read();
                    int ACC_ID = Convert.ToInt32(dr[0].ToString()) + 1;
                    dr.Close();
                    cmd2.Dispose();

                    OracleCommand cmd_acc_lnk = new OracleCommand(
                        "INSERT INTO user_acc_link_jsb (user_id,acc_no,acc_sts,acc_type,acc_app_type,acc_curr,acc_branch,acc_primary,acc_status,acc_id,iban) " +
                        "VALUES (:userlog,:accountNo,NULL,:accountType,:accAppType,:currencyCode,:branchCode,:accPrimary,'A',:accId,:iban)", con);
                    cmd_acc_lnk.Parameters.Add("userlog", OracleType.VarChar).Value = userlog;
                    cmd_acc_lnk.Parameters.Add("accountNo", OracleType.VarChar).Value = Account_No;
                    cmd_acc_lnk.Parameters.Add("accountType", OracleType.VarChar).Value = Account_Type_Code;
                    cmd_acc_lnk.Parameters.Add("accAppType", OracleType.VarChar).Value = acc_app_type;
                    cmd_acc_lnk.Parameters.Add("currencyCode", OracleType.VarChar).Value = Currency_Code;
                    cmd_acc_lnk.Parameters.Add("branchCode", OracleType.VarChar).Value = Branch_Code;
                    cmd_acc_lnk.Parameters.Add("accPrimary", OracleType.VarChar).Value = acc_prim ? "True" : "False";
                    cmd_acc_lnk.Parameters.Add("accId", OracleType.Int32).Value = ACC_ID;
                    cmd_acc_lnk.Parameters.Add("iban", OracleType.VarChar).Value = IBAN;
                    cmd_acc_lnk.ExecuteNonQuery();

                    lblconfirm = "Account Added Successfully";
                    con.Close();
                }
                catch (Exception ex)
                {
                    lblconfirm = "System Error : " + ex.Message;
                }
            }
            return lblconfirm;
        }

        public string checkaccountifbound(string accountnumber, string userid)
        {
            OracleCommand cmd;
            OracleDataReader dr;
            using (OracleConnection con = new OracleConnection(conString))
            {
                con.Open();
                cmd = new OracleCommand("select count(*) from user_acc_link where acc_no = :accountnumber and user_id = :userid", con);
                cmd.Parameters.Add("accountnumber", OracleType.VarChar).Value = accountnumber;
                cmd.Parameters.Add("userid", OracleType.VarChar).Value = userid;
                dr = cmd.ExecuteReader();
                dr.Read();
                int counter;
                counter = Convert.ToInt32(dr[0].ToString());
                con.Close();
                dr.Close();
                if (counter == 1)
                {
                    return "Account already exists";
                }
                else
                {
                    return "Account available";
                }
            }
        }
        public string checkuser(string accountnumber)
        {
            string user = null;
            using (OracleConnection con = new OracleConnection(conString))
            {
                con.Open();
                OracleCommand cmd = new OracleCommand("select user_log from users_jsb where def_acc = :accountnumber", con);
                cmd.Parameters.Add("accountnumber", OracleType.VarChar).Value = accountnumber;

                OracleDataReader dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {
                    user = dr[0].ToString();
                }

                dr.Close();
                con.Close();
            }
            return user;
        }


        public string checkuserforcorp(string accountnumber, string cat)
        {
            string user = null;
            using (OracleConnection con = new OracleConnection(conString))
            {
                con.Open();

                string query = "select user_log from users_jsb where def_acc = :accountnumber";
                if (cat == "2")
                {
                    query += " and user_log like '%O%'";
                }
                else if (cat == "3")
                {
                    query += " and user_log like '%A%'";
                }

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("accountnumber", OracleType.VarChar).Value = accountnumber;

                OracleDataReader dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {
                    user = dr[0].ToString();
                }

                dr.Close();
                con.Close();
            }
            return user;
        }


        public string checkuserbyrim(string accountnumber)
        {
            string user = null;
            OracleCommand cmd;
            OracleDataReader dr;
            using (OracleConnection con = new OracleConnection(conString))
            {
                con.Open();
                cmd = new OracleCommand("select user_log from users_jsb where user_rim = :accountnumber", con);
                cmd.Parameters.Add("accountnumber", OracleType.VarChar).Value = accountnumber;
                dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {
                    user = dr[0].ToString();
                }
                con.Close();
                dr.Close();

            }
            return user;

        }


        public string checkuserforcorpbyrim(string accountnumber, string cat)
        {

            string cato = "";
            if (cat == "2")
            {
                cato = "and user_log like  '%O%'";

            }

            if (cat == "3")
            {
                cato = "and user_log like  '%A%'";

            }
            string user = null;
            OracleCommand cmd;
            OracleDataReader dr;
            using (OracleConnection con = new OracleConnection(conString))
            {
                con.Open();
                string query = "select user_log from users_jsb where user_rim = :accountnumber";
                if (cat == "2")
                {
                    query += " and user_log like '%O%'";
                }
                if (cat == "3")
                {
                    query += " and user_log like '%A%'";
                }
                cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("accountnumber", OracleType.VarChar).Value = accountnumber;
                dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {
                    user = dr[0].ToString();
                }
                con.Close();
                dr.Close();

            }
            return user;

        }


        public string getcustomerfullname(string primaryaccount)
        {
            string customername = "N/A";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select user_name from users where def_acc = '" + primaryaccount + "'";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            customername = sdr["user_name"].ToString();
                        }
                    }
                    con.Close();
                }
            }
            return customername;
        }

        public List<pendingacts> Pendingacounts(String bracode)
        {
            OracleCommand cmd;
            OracleDataReader dr;

            String userid, username, useract, newuseract, newuseractcomplete;
            String query1, result;

            List<pendingacts> customer = new List<pendingacts>();

            if (!bracode.Equals("000"))
            {
                query1 = "select user_acc_link.user_id,users.user_name,b.branch_name||' - '||t.act_name||' - '||c.curr_name||' - '||SUBSTR(def_acc,2) def_acc,(select branch_name  from branchs where branch_code =SUBSTR(ACC_NO,3,3))||' - '||(select act_name  from act_types where act_type_code =SUBSTR(ACC_NO,6,5))||' - '||(select curr_name  from currency where  CURR_STS='1' and  CURR_CODE =SUBSTR(ACC_NO,11,3))||' - '||SUBSTR(ACC_NO,14) as account_to_be_added, ACC_NO from users , user_acc_link ,branchs b ,act_types t , currency c ,security_master sm where c.CURR_STS='1' and SUBSTR(account,3,3)=b.branch_code and SUBSTR(account,6,5)=t.act_type_code and SUBSTR(account,11,3)=c.CURR_CODE and ACC_STATUS='P' and user_acc_link.user_id=users.user_id and users.created_by = sm.user_log and sm.user_branch = '" + bracode + "' order by user_id";
            }
            else
            {
                query1 = "select user_acc_link.user_id,users.user_name,b.branch_name||' - '||t.act_name||' - '||c.curr_name||' - '||SUBSTR(def_acc,2) def_acc,(select branch_name  from branchs where branch_code =SUBSTR(ACC_NO,3,3))||' - '||(select act_name  from act_types where act_type_code =SUBSTR(ACC_NO,6,5))||' - '||(select curr_name  from currency where  CURR_STS='1' and  CURR_CODE =SUBSTR(ACC_NO,11,3))||' - '||SUBSTR(ACC_NO,14) as account_to_be_added, ACC_NO from users , user_acc_link ,branchs b ,act_types t , currency c ,security_master sm where c.CURR_STS='1' and SUBSTR(account,3,3)=b.branch_code and SUBSTR(account,6,5)=t.act_type_code and SUBSTR(account,11,3)=c.CURR_CODE and ACC_STATUS='P' and user_acc_link.user_id=users.user_id and users.created_by = sm.user_log order by user_id";
            }

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        userid = dr[0].ToString();
                        username = dr[1].ToString();
                        useract = dr[2].ToString();
                        newuseract = dr[3].ToString();
                        newuseractcomplete = dr[4].ToString();

                        customer.Add(new pendingacts
                        {
                            USER_ID = userid,
                            USER_NAME = username,
                            DEF_ACC = useract,
                            ACC_NO = newuseract,
                            ACC_NO1 = newuseractcomplete
                        });
                    }
                }


            }


            return customer;
        }

        public List<pendingacts> AllPendingAccounts()
        {
            OracleCommand cmd;
            OracleDataReader dr;

            String userid, username, useract, newuseract, newuseractcomplete;
            String query1, result;

            List<pendingacts> customer = new List<pendingacts>();

            query1 = "select user_acc_link.user_id,users.user_name,b.branch_name||' - '||t.act_name||' - '||c.curr_name||' - '||SUBSTR(def_acc,2) def_acc,(select branch_name  from branchs where branch_code =SUBSTR(ACC_NO,3,3))||' - '||(select act_name  from act_types where act_type_code =SUBSTR(ACC_NO,6,5))||' - '||(select curr_name  from currency where  CURR_STS='1' and  CURR_CODE =SUBSTR(ACC_NO,11,3))||' - '||SUBSTR(ACC_NO,14) as account_to_be_added, ACC_NO from users , user_acc_link ,branchs b ,act_types t , currency c where c.CURR_STS='1' and SUBSTR(account,3,3)=b.branch_code and SUBSTR(account,6,5)=t.act_type_code and SUBSTR(account,11,3)=c.CURR_CODE and ACC_STATUS='P' and user_acc_link.user_id=users.user_id order by user_id";

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        userid = dr[0].ToString();
                        username = dr[1].ToString();
                        useract = dr[2].ToString();
                        newuseract = dr[3].ToString();
                        newuseractcomplete = dr[4].ToString();

                        customer.Add(new pendingacts
                        {
                            USER_ID = userid,
                            USER_NAME = username,
                            DEF_ACC = useract,
                            ACC_NO = newuseract,
                            ACC_NO1 = newuseractcomplete
                        });
                    }
                }


            }


            return customer;
        }



        public List<actAuthorizationinfo> newactAuthorizationinfo(string userid, string act)
        {

            OracleCommand cmd;
            OracleDataReader dr;

            String username, useract;
            String query1, result;
            List<actAuthorizationinfo> customer = new List<actAuthorizationinfo>();
            OracleDataReader dr3;
            OracleCommand cmd3;
            string sqstr;
            string msg = "";
            string br, Sessioncurr = "";
            String acc = "";
            String acc_type = "";
            String acc_no;
            String curr;
            String curr_name = "";
            String lang;
            String brname = "";
            String acctype = "";
            String roleid = "", profilename = "";
            query1 = "select user_acc_link.user_id,user_name,def_acc,ACC_NO from users , user_acc_link where user_acc_link.user_id='" + userid + "' and users.user_id='" + userid + "' and ACC_NO='" + act + "' and user_acc_link.acc_status='P'";
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd3 = new OracleCommand(query1, con);

                con.Open();


                dr3 = cmd3.ExecuteReader();
                if (dr3.HasRows)
                {
                    while (dr3.Read())
                    {
                        // 'lb_cust_name.Text = dr3(1)
                        OracleDataReader dr2;
                        OracleDataReader dr4;
                        OracleDataReader dr5;
                        OracleDataReader dr6;
                        OracleCommand cmd2;
                        OracleCommand cmd4;
                        OracleCommand cmd5;
                        OracleCommand cmd6;


                        acc = dr3[3].ToString();
                        br = acc.Substring(2, 3);
                        acc_type = acc.Substring(5, 5);
                        Sessioncurr = acc.Substring(10, 3);
                        acc_no = acc.Substring(13);

                        cmd4 = new OracleCommand(("select BRANCH_NAME from BRANCHS where BRANCH_CODE_NO='" + br + "'"), con);
                        dr4 = cmd4.ExecuteReader();
                        if (dr4.Read())
                        {
                            brname = dr4[0].ToString();

                        }

                        dr4.Close();
                        cmd5 = new OracleCommand(("select act_name from act_types where act_type_code ='" + (acc_type + "'")), con);
                        dr5 = cmd5.ExecuteReader();
                        if (dr5.HasRows)
                        {
                            dr5.Read();
                            acctype = dr5[0].ToString();

                        }
                        else
                        {
                            cmd5 = new OracleCommand(("select act_name from invact_types where act_type_code='" + (acc_type + "'")), con);
                            dr5 = cmd5.ExecuteReader();
                            if (dr5.HasRows)
                            {
                                dr5.Read();
                                acctype = dr5[0].ToString();

                            }
                            else
                                acctype = "Account Type Not Found";

                        }

                        dr5.Close();

                        cmd6 = new OracleCommand(("select CURR_NAME from CURRENCY where  CURR_STS='1' and  CURR_CODE = '" + Sessioncurr + "'"), con);
                        dr6 = cmd6.ExecuteReader();
                        if (dr6.Read())
                        {
                            curr_name = dr6[0].ToString();

                        }

                        dr6.Close();

                        customer.Add(new actAuthorizationinfo
                        {
                            userid = dr3[0].ToString(),
                            Branch = brname,
                            AccountType = acctype,
                            Customername = dr3[1].ToString(),
                            Currency = curr_name,
                            Customeraccount = acc.Substring(12, 7),
                            completeact = acc,
                        });
                    }
                    dr3.Close();
                }
            }
            return customer;
        }

        public int updateAccount(String userid, String account, String status)
        {
            OracleCommand cmd;
            int result = -1;


            String query1;
            query1 = "update user_acc_link set ACC_STATUS='" + status + "', ACC_STS='" + status + "' where  ACC_no='" + account + "' and  user_id ='" + userid + "'";
            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();
                result = cmd.ExecuteNonQuery();
            }
            return result;
        }
        public List<GETpassword> getpassword(String account)
        {
            OracleCommand cmd;
            OracleDataReader dr;
            String acttypename = "", acttype;
            List<GETpassword> list = new List<GETpassword>();
            string enc_pwd = "", br, branchname = "", lblconfirm = "System Error", pass, name = "";
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd4, cmd5;

                OracleDataReader dr4, dr5;

                try
                {

                    br = account.Substring(2, 3);
                    acttype = account.Substring(5, 5);


                    cmd4 = new OracleCommand("select BRANCH_NAME from BRANCHS where BRANCH_CODE_NO='" + br + "'", con);
                    con.Open();
                    dr4 = cmd4.ExecuteReader();
                    if (dr4.Read())
                    {
                        branchname = dr4[0].ToString();
                    }

                    cmd5 = new OracleCommand("select act_name from act_types  where act_type_code='" + acttype + "'", con);
                    dr5 = cmd5.ExecuteReader();
                    if (dr5.Read())
                    {
                        acttypename = dr5[0].ToString();
                    }


                    cmd = new OracleCommand("select USER_PAS,DEF_ACC ,USER_NAME from users where DEF_ACC='" + account + "'", con);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        enc_pwd = dr[0].ToString();
                        pass = enc_pwd;
                        account = dr[1].ToString().Substring(2, 11);
                        name = dr[2].ToString();
                        lblconfirm = "Successfully";
                    }
                    else
                    {
                        lblconfirm = "Wrong Customer Account";
                    }

                }
                catch (Exception ex)
                {
                    lblconfirm = "System Error";
                }
                list.Add(new GETpassword
                {
                    pass = enc_pwd,
                    name = name,
                    account = account,
                    lblconfirm = lblconfirm,
                    branchname = branchname,
                });
                return list;
            }
        }

        public String resetpassword(String user_log)
        {
            string lblconfirm = "System Error", pass = "";

            using (OracleConnection con = new OracleConnection(conString))
            {
                try
                {
                    con.Open();

                    // WAPT01-05: Parameterized — no string concatenation
                    OracleCommand cmd = new OracleCommand(
                        "SELECT otp_pwd FROM users_jsb WHERE user_log = :user_log", con);
                    cmd.Parameters.Add("user_log", OracleType.VarChar).Value = user_log;

                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        pass = dr[0].ToString();
                        lblconfirm = "Successfully";
                    }
                    else
                    {
                        lblconfirm = "Pleace Check Your Account";
                        pass = "0";
                    }
                }
                catch (Exception ex)
                {
                    lblconfirm = "System Error";
                    pass = "0";
                }

                return pass;
            }
        }

        

        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }


        public custinfo getcustinfo(String userlog, String accountnumber)
        {
            Boolean FLAG;
            String lblconfirm = "";
            OracleCommand cmd;
            OracleDataReader dr;
            int counter;
            custinfo model = new custinfo();
            //String query1 = "select  u.user_id, u.user_name,u.user_log,u.user_pwd,u.user_email,u.user_mobile,u.user_adrs,m.name,u.user_status" +
            // " from users u, tbl_rolemaster m  where u.roleid=m.roleid and u.DEF_ACC='13" + branchcode + acttype + acc_curr + acc_no + "' and  catogry ='"+category+"'";
            String query1 = "select *  from users_jsb where  user_log = '" + userlog + "' or def_acc = '" + accountnumber + "'";


            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);
                con.Open();
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        //model.user_id = dr[0].ToString();
                        //model.user_name = dr[1].ToString();
                        //model.user_log = dr[2].ToString();
                        //model.user_pwd = dr[3].ToString();
                        //model.user_email = dr[4].ToString();
                        //model.user_adrs = dr[6].ToString();
                        //model.user_mobile = dr[5].ToString();
                        //model.name = dr[7].ToString();
                        //model.status = dr[8].ToString();
                        model.user_id = dr["USER_ID"].ToString();
                        model.user_name = dr["USER_NAME_EN"].ToString();
                        model.user_log = dr["USER_LOG"].ToString();
                        model.user_pwd = dr["USER_PWD"].ToString();
                        model.user_email = dr["USER_EMAIL"].ToString();
                        model.user_adrs = dr["USER_ADDRESS"].ToString();
                        model.user_mobile = dr["USER_MOBILE"].ToString();
                        model.role_id = dr["roleid"].ToString();
                        model.status = dr["USER_STATUS"].ToString();
                        model.def_account = dr["DEF_ACC"].ToString();
                        model.lblconfirm = "This Account is Already exist";
                    }
                }
                else
                {
                    model.lblconfirm = "This Account is available";
                }
            }
            return model;
        }

        public custinfo getcustinfobyid(String id)
        {
            Boolean FLAG;
            String lblconfirm = "";
            OracleCommand cmd;
            OracleDataReader dr;
            int counter;
            custinfo model = new custinfo();
            //String query1 = "select  u.user_id, u.user_name,u.user_log,u.user_pwd,u.user_email,u.user_mobile,u.user_adrs,m.name,u.user_status" +
            // " from users u, tbl_rolemaster m  where u.roleid=m.roleid and u.DEF_ACC='13" + branchcode + acttype + acc_curr + acc_no + "' and  catogry ='"+category+"'";
            String query1 = "select * from users_jsb where user_log = '" + id + "'";


            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);
                con.Open();
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        //model.user_id = dr[0].ToString();
                        //model.user_name = dr[1].ToString();
                        //model.user_log = dr[2].ToString();
                        //model.user_pwd = dr[3].ToString();
                        //model.user_email = dr[4].ToString();
                        //model.user_adrs = dr[6].ToString();
                        //model.user_mobile = dr[5].ToString();
                        //model.name = dr[7].ToString();
                        //model.status = dr[8].ToString();
                        model.user_id = dr["USER_ID"].ToString();
                        model.user_name = dr["USER_NAME_EN"].ToString();
                        model.user_log = dr["USER_LOG"].ToString();
                        model.user_pwd = dr["USER_PWD"].ToString();
                        model.user_email = dr["USER_EMAIL"].ToString();
                        model.user_adrs = dr["USER_ADDRESS"].ToString();
                        model.user_mobile = dr["USER_MOBILE"].ToString();
                        model.role_id = dr["roleid"].ToString();
                        model.status = dr["USER_STATUS"].ToString();
                        model.def_account = dr["DEF_ACC"].ToString();
                        model.lblconfirm = "This Account is Already exist";
                    }
                }
                else
                {
                    model.lblconfirm = "This Account is available";
                }
            }
            return model;
        }

        public int Updatecustomer(int n, string userlog , string email)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "update users_jsb   set user_profile_id = "+ n + " , user_email = '" + email+"'   where user_log= '" + userlog + "' ";
                OracleCommand cmd = new OracleCommand(query, con);
                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                return result;
            }
        }
        public List<pageparameter> PopulateProfilemangement(String categoryid)
        {
            int i = 0; ;
            List<pageparameter> items = new List<pageparameter>();
            using (OracleConnection con = new OracleConnection(conString))
            {

                //string query = "select  t.menuid ,t.menuname,t.menu_ar_name , tm.menuname  parnet_name,tm.menu_ar_name parnet_name_ar,tm.menuid  parnet_id from (select  menu_ar_name,  menuname,menuid  from cpanel_menumaster where MENUPARENTId=0  ) tm ,cpanel_menumaster t    where t.MENUPARENTID<>0 and t.menuparentid=tm.menuid  and menu_category in ('" + categoryid + "','1')  order by menuid ,menuparentid";
                string query = " select  t.menu_id ,t.menu_name,t.menu_url, tm.menu_name parnet_name,tm.menu_name_ar parnet_name_ar,tm.menu_id parnet_id from (select  menu_name_ar,  menu_name,menu_id  from jsb_menu_master where MENU_PARENT_Id=0  ) tm ,jsb_menu_master t where t.MENU_PARENT_ID<>0 and t.menu_parent_id=tm.menu_id order by menu_id ,menu_parent_id";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new pageparameter
                            {
                                menuid = sdr[0].ToString(),
                                menuname = sdr[1].ToString(),
                                menuurl = sdr[2].ToString(),
                                //Parent_menuname = sdr[3].ToString(),
                                //Parent_menuname_ar = sdr[4].ToString(),

                                menuparentid = sdr[5].ToString(),
                                IsSelected = false,
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;

        }

        public List<pageparameter> PopulateCustomerProfilemangement(String categoryid)
        {
            int i = 0; ;
            List<pageparameter> items = new List<pageparameter>();
            using (OracleConnection con = new OracleConnection(conString))
            {

                string query = "select  t.menuid ,t.menuname,t.menu_ar_name , tm.menuname  parnet_name,tm.menu_ar_name parnet_name_ar,tm.menuid  parnet_id from (select  menu_ar_name,  menuname,menuid  from tbl_menumaster where MENUPARENTId=0  ) tm ,tbl_menumaster t    where t.MENUPARENTID<>0 and t.menuparentid=tm.menuid  and menu_category in ('" + categoryid + "','0')  order by menuid ,menuparentid";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new pageparameter
                            {
                                menuid = sdr[0].ToString(),
                                menuname = sdr[1].ToString(),
                                //menuname_ar = sdr[2].ToString(),
                                //Parent_menuname = sdr[3].ToString(),
                                //Parent_menuname_ar = sdr[4].ToString(),

                                menuparentid = sdr[5].ToString(),
                                IsSelected = false,
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        public int deletecustomer(string uid)
        {

            using (OracleConnection con = new OracleConnection(conString))
            {

                OracleCommand cmd = new OracleCommand("", con);
                cmd.CommandText = "DELETECUSTOMER";
                cmd.CommandType = CommandType.StoredProcedure;

                OracleParameter p3 = new OracleParameter("status", OracleType.VarChar, 2000);
                p3.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p3);

                //cmd.Parameters.Add("uid", OracleType.Int32).Value = int.Parse(uid);

                cmd.Parameters.Add("uid", OracleType.VarChar).Value = uid;


                con.Open();
                int result = -1;

                result = cmd.ExecuteNonQuery();

                if (p3.Value.ToString() == "Fail")
                {
                    result = -1;
                }

                return result;
            }
        }

        public string getuserid(string username)
        {
            string userid = "";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select user_id from users_jsb where user_log = '" + username + "'";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();

                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        if (dataReader["user_id"] != DBNull.Value)
                        {
                            userid = dataReader["user_id"].ToString();
                        }
                    }
                    return userid;
                }
            }
        }


        public List<SelectListItem> GetGatgories()

        {
            List<SelectListItem> items = new List<SelectListItem>();



            int i = 0;

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select  cat_id,cat_name from category";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {

                            //items.Add(new SelectListItem
                            //{
                            //    Text = " Select Customer category ",
                            //    Value = "0",
                            //});

                            while (sdr.Read())
                            {

                                items.Add(new SelectListItem
                                {
                                    Text = sdr[1].ToString(),
                                    Value = sdr[0].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }

            return items;

        }

        public String deleteaccount(String act, String account, String category)
        {
            String lblconfirm = "System Error", user_id = null;
            bool FLAG;
            OracleCommand cmd;
            OracleDataReader dr;
            OracleCommand cmd2;
            OracleCommand cmd_acc_lnk;
            OracleCommand delete_cmd;
            using (OracleConnection con = new OracleConnection(conString))
            {
                try
                {
                    con.Open();
                    user_id = getCustIDFromAcc(act);
                    cmd = new OracleCommand("select count(*) from user_acc_link where acc_no='" + account + "' and user_id='" + user_id + "'", con);
                    dr = cmd.ExecuteReader();
                    dr.Read();
                    int counter;
                    counter = Convert.ToInt32(dr[0].ToString());
                    dr.Close();
                    cmd.Dispose();
                    if (counter == 1)
                    {
                        String dp_branch, dp_acc_tybe, dp_acc_curr;
                        dp_acc_tybe = account.Substring(5, 5);
                        dp_branch = account.Substring(2, 3);
                        dp_acc_curr = account.Substring(10, 3);
                        String sql2 = "select  nvl(max (acc_id),0) from deleted_user_acc_link where user_id=" + user_id;
                        cmd2 = new OracleCommand(sql2, con);
                        dr = cmd2.ExecuteReader();
                        dr.Read();
                        int ACC_ID;
                        ACC_ID = Convert.ToInt32(dr[0].ToString());
                        dr.Close();
                        cmd2.Dispose();
                        ACC_ID = ACC_ID + 1;
                        delete_cmd = new OracleCommand("delete from user_acc_link where user_id = '" + user_id + "' and acc_no = '" + account + "'", con);
                        delete_cmd.ExecuteNonQuery();
                        cmd_acc_lnk = new OracleCommand("INSERT INTO deleted_user_acc_link (BRANCH_CODE,ACT_TYPE,USER_ID,ACC_NO,ACC_STS,ACC_CURR,ACC_LANG,ACC_STATUS,ACC_ID,CATOGRY) values ('"
                                            + dp_branch + "','" + dp_acc_tybe + "','" + user_id + "','" + account + "','D','" + dp_acc_curr + "','AR','D','" + ACC_ID + "',  '" + category + "')", con);
                        cmd_acc_lnk.ExecuteNonQuery();
                        lblconfirm = "Account Deleted Successfully";
                    }
                    else
                    {
                        lblconfirm = "These Account Already exist";
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    lblconfirm = "System Error";
                }
            }
            return lblconfirm;
        }

        public Boolean checkaccountbelongstouser(string userid, string accountnumber)
        {
            Boolean result = false;
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select * from user_acc_link where user_id = '" + userid + "' and acc_no = '" + accountnumber + "'";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            result = true;
                        }
                    }
                    con.Close();
                }
            }
            return result;
        }

        public Boolean checkaccountuser(string type, string accountnumber, string userlog)
        {
            Boolean result = false;
            using (OracleConnection con = new OracleConnection(conString))
            {
                using (OracleCommand cmd = new OracleCommand("select * from user_acc_link_jsb where acc_type = :type and acc_no = :accountnumber and user_id = :userlog"))
                {
                    cmd.Connection = con;
                    cmd.Parameters.Add("type", OracleType.VarChar).Value = type;
                    cmd.Parameters.Add("accountnumber", OracleType.VarChar).Value = accountnumber;
                    cmd.Parameters.Add("userlog", OracleType.VarChar).Value = userlog;

                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        public Boolean checkaccountuserforcorp(string type, string accountnumber, string userlog)
        {
            Boolean result = false;
            using (OracleConnection con = new OracleConnection(conString))
            {
                using (OracleCommand cmd = new OracleCommand("select * from user_acc_link_jsb where acc_type = :type and acc_no = :accountnumber and user_id = :userlog"))
                {
                    cmd.Connection = con;
                    cmd.Parameters.Add("type", OracleType.VarChar).Value = type;
                    cmd.Parameters.Add("accountnumber", OracleType.VarChar).Value = accountnumber;
                    cmd.Parameters.Add("userlog", OracleType.VarChar).Value = userlog;

                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        public custinfo getcustinfo(String branchcode, String acttype, String acc_no, String acc_curr, String category)
        {
            Boolean FLAG;
            String lblconfirm = "";
            OracleCommand cmd;
            OracleDataReader dr;
            int counter;
            custinfo model = new custinfo();
            String query1 = "select  u.user_id, u.user_name,u.user_log,u.user_pwd,u.user_email,u.user_mobile,u.user_adrs,m.name,u.user_status" +
            " from users u, tbl_rolemaster m  where u.roleid=m.roleid and u.DEF_ACC='23" + branchcode + acttype + acc_curr + acc_no + "' and  catogry ='" + category + "'";

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        model.user_id = dr[0].ToString();
                        model.user_name = dr[1].ToString();
                        model.user_log = dr[2].ToString();
                        model.user_pwd = dr[3].ToString();
                        model.user_email = dr[4].ToString();
                        model.user_adrs = dr[6].ToString();
                        model.user_mobile = dr[5].ToString();
                        model.name = dr[7].ToString();
                        model.status = dr[8].ToString();
                        model.lblconfirm = "This Account is Already exist";

                    }

                }


                else
                {
                    model.lblconfirm = "This Account is available";
                }




            }
            return model;
        }

        public Customerinfopass GetUserinfoData(string idorname)
        {
            Customerinfopass usermodel = new Customerinfopass();
            //char[] chararray = idorname.ToCharArray();
            //if (char.IsDigit(chararray[0]) && idorname.Length == 12)
            //{
            using (OracleConnection con = new OracleConnection(conString))
            {
                // WAPT01-05: Parameterized — no string concatenation
                OracleCommand cmd = new OracleCommand(
                    "SELECT user_name_en, def_acc AS account_number, user_status FROM users_jsb WHERE user_log = :idorname", con);
                cmd.Parameters.Add("idorname", OracleType.VarChar).Value = idorname;

                con.Open();

                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        //usermodel.BranchCode = dataReader["branch_code"].ToString();
                        //usermodel.Branch = dataReader["branch_name"].ToString();
                        //usermodel.CurrencyCode = dataReader["currency_code"].ToString();
                        //usermodel.Currency = dataReader["currency_name"].ToString();
                        //usermodel.AccountTypecode = dataReader["account_type_code"].ToString();
                        //usermodel.AccountType = dataReader["account_type"].ToString();
                        usermodel.AccountNumber = dataReader["account_number"].ToString();
                        //usermodel.CategoryCode = dataReader["category_id"].ToString();
                        //usermodel.category = dataReader["category_name"].ToString();
                        usermodel.CustomerName = dataReader["user_name_en"].ToString();
                        usermodel.status = dataReader["user_status"].ToString();
                        usermodel.CustomerID = idorname;
                    }
                }
                return usermodel;
            }
            //}
            //else if (char.IsDigit(chararray[0]))
            //{
            //    using (OracleConnection con = new OracleConnection(conString))
            //    {
            //        //string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_name, SUBSTR(def_acc,6,5) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,5)) as account_type,SUBSTR(def_acc,11,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name from users where user_id = '" + int.Parse(idorname) + "'";
            //        string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_name, SUBSTR(def_acc,6,6) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,6)) as account_type,SUBSTR(def_acc,12,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name,SUBSTR(def_acc,19,2) as subno, SUBSTR(def_acc,23,3) as subgl from users where user_id = '" + idorname + "'";
            //        OracleCommand cmd = new OracleCommand(query, con);
            //        con.Open();
            //        using (IDataReader dataReader = cmd.ExecuteReader())
            //        {
            //            while (dataReader.Read())
            //            {
            //                usermodel.BranchCode = dataReader["branch_code"].ToString();
            //                usermodel.Branch = dataReader["branch_name"].ToString();
            //                usermodel.CurrencyCode = dataReader["currency_code"].ToString();
            //                usermodel.Currency = dataReader["currency_name"].ToString();
            //                usermodel.AccountTypecode = dataReader["account_type_code"].ToString();
            //                usermodel.AccountType = dataReader["account_type"].ToString();
            //                usermodel.AccountNumber = dataReader["account_number"].ToString();
            //                usermodel.CategoryCode = dataReader["category_id"].ToString();
            //                usermodel.category = dataReader["category_name"].ToString();
            //                usermodel.SUBNO = dataReader["subno"].ToString();
            //                usermodel.SUBGL = dataReader["subgl"].ToString();
            //                usermodel.CustomerID = idorname;
            //            }
            //        }
            //        return usermodel;
            //    }
            //}
            //else
            //{
            //    using (OracleConnection con = new OracleConnection(conString))
            //    {
            //        //string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_name, SUBSTR(def_acc,6,5) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,5)) as account_type,SUBSTR(def_acc,11,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name from users where user_log = '" + idorname + "'";
            //        string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_name, SUBSTR(def_acc,6,6) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,6)) as account_type,SUBSTR(def_acc,12,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name,SUBSTR(def_acc,19,2) as subno, SUBSTR(def_acc,23,3) as subgl from users where user_log = '" + idorname + "'";
            //        OracleCommand cmd = new OracleCommand(query, con);
            //        con.Open();
            //        using (IDataReader dataReader = cmd.ExecuteReader())
            //        {
            //            while (dataReader.Read())
            //            {
            //                usermodel.BranchCode = dataReader["branch_code"].ToString();
            //                usermodel.Branch = dataReader["branch_name"].ToString();
            //                usermodel.CurrencyCode = dataReader["currency_code"].ToString();
            //                usermodel.Currency = dataReader["currency_name"].ToString();
            //                usermodel.AccountTypecode = dataReader["account_type_code"].ToString();
            //                usermodel.AccountType = dataReader["account_type"].ToString();
            //                usermodel.AccountNumber = dataReader["account_number"].ToString();
            //                usermodel.CategoryCode = dataReader["category_id"].ToString();
            //                usermodel.category = dataReader["category_name"].ToString();
            //                usermodel.SUBNO = dataReader["subno"].ToString();
            //                usermodel.SUBGL = dataReader["subgl"].ToString();
            //                usermodel.CustomerID = idorname;
            //            }
            //        }
            //        return usermodel;
            //    }
            //}
        }

        public List<SelectListItem> CPanel_GetGatgories()
        {
            List<SelectListItem> items = new List<SelectListItem>();



            int i = 0;

            using (OracleConnection con = new OracleConnection(conString))
            {
                //string query = " select  cat_id,cat_name from CPANEL_CATEGORY";
                string query = " select  cat_id,cat_name from JSB_CPANEL_CATEGORY";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {

                            items.Add(new SelectListItem
                            {
                                Text = "-- Select Customer category --",
                                Value = "0",
                            });
                            while (sdr.Read())
                            {

                                items.Add(new SelectListItem
                                {
                                    Text = sdr[1].ToString(),
                                    Value = sdr[0].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }

            return items;

        }

        public Custreport GetCustomerReportData(string idorname)
        {
            Custreport usermodel = new Custreport();

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select (select branch_name_en from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name,(select curr_name from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_name, (select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,6)) as account_type,SUBSTR(def_acc,2,11) as account_number, (select cat_name from category where cat_id = users.catogry) as category_name,user_name,user_log,user_email,user_mobile,decode(user_status,'A','Active','D','Deactive','R','Rejected','P','Pending','DE','Deleted','U','Unauthorized') as user_status,last_login,wrong_password from users_jsb where user_id = '" + int.Parse(idorname) + "' or user_log = '" + idorname + "' or user_mobile = '" + idorname + "' or  SUBSTR(users.def_acc,14,5) = '" + idorname + "'";
                OracleCommand cmd = new OracleCommand(query, con);
                con.Open();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        usermodel.Branch = dataReader["branch_name_en"].ToString();
                        usermodel.Currency = dataReader["currency_name"].ToString();
                        usermodel.AccountType = dataReader["account_type"].ToString();
                        usermodel.AccountNumber = dataReader["account_number"].ToString();
                        usermodel.category = dataReader["category_name"].ToString();
                        usermodel.CustomerName = dataReader["user_name"].ToString();
                        usermodel.username = dataReader["user_log"].ToString();
                        usermodel.user_email = dataReader["user_email"].ToString();
                        usermodel.phonenumber = dataReader["user_mobile"].ToString();
                        usermodel.CustStatus = dataReader["user_status"].ToString();
                        usermodel.lastlogin = dataReader["last_login"].ToString();
                        usermodel.wrong_passwords = dataReader["wrong_password"].ToString();
                        usermodel.CustomerID = idorname;
                    }
                }
                return usermodel;
            }
        }

        public List<String> GetCustomerLinkedAccounts(string CustomerID)
        {
            List<string> stringlist = new List<string>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select decode(acc_status,'P','Pending','A','Active','B','Blocked','U','Authorized','D','Deactive','DE','Deleted','R','Rejected','N/A') as status,'-' as separator,(select branch_name from branchs where branch_code = SUBSTR(user_acc_link.acc_no,3,3)) as branch_name,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(user_acc_link.acc_no,6,5)) as account_type,SUBSTR(acc_no,14) as account_number from user_acc_link where user_id = '" + int.Parse(CustomerID) + "'";
                OracleCommand cmd = new OracleCommand(query, con);
                con.Open();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        stringlist.Add(dataReader["status"].ToString() + " - " + dataReader["branch_name"].ToString() + " - " + dataReader["account_type"].ToString() + " - " + dataReader["account_number"].ToString());
                    }
                }
                return stringlist;
            }
        }

        public string GetCustomerChannels(string CustomerID)
        {
            string mobileholder = "F", ibankingholder = "F";

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select user_ebank,user_emobile from user_channel where userid = '" + int.Parse(CustomerID) + "'";
                OracleCommand cmd = new OracleCommand(query, con);
                con.Open();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        mobileholder = dataReader["user_ebank"].ToString();
                        ibankingholder = dataReader["user_emobile"].ToString();
                    }
                }
            }

            if (mobileholder == "T" && ibankingholder == "T")
            {
                return "3";
            }
            else if (mobileholder == "F" && ibankingholder == "T")
            {
                return "2";
            }
            else if (mobileholder == "T" && ibankingholder == "F")
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        public List<profilesparameter> GetProfiles()
        {
            int i = 0; ;
            List<profilesparameter> items = new List<profilesparameter>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select t.roleid,name,DECODE (t.active,'1','Active','DeActive') status  from tbl_rolemaster t where t.name!='Admin' order by t.roleid";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new profilesparameter
                            {
                                profielid = sdr[0].ToString(),
                                profilename = sdr[1].ToString(),
                                profilestatus = sdr[2].ToString(),

                                IsSelected = false,
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }
        public List<SelectListItem> PopulateCustStatus()
        {
            List<SelectListItem> items = new List<SelectListItem>();



            int i = 0;

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select  STATUS_CODE,STATUS_NAME from CUSTOMERSTATUS where ACTIVE='1'";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {

                            items.Add(new SelectListItem
                            {
                                Text = "-- Select Customer Status --",
                                Value = "0",
                            });
                            items.Add(new SelectListItem
                            {
                                Text = "All Statuses",
                                Value = "All",
                            });
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr[1].ToString(),
                                    Value = sdr[0].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }

            return items;

        }

        internal string cpancel_deleteexitingrole(string roleid)
        {
            string lblconfirm = "";
            OracleDataReader dr;
            OracleCommand cmd_acc_lnk;
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd;
                try
                {
                    con.Open();
                    cmd = new OracleCommand("delete from jsb_role_menu_mapping where mapping_role_id = '" + roleid + "'", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                catch (Exception ex)
                {
                    lblconfirm = "System Error - " + ex.Message;
                }
            }
            return lblconfirm;
        }

        public List<Menu> GetRoleMenu(string rolenumber)
        {
            /* using ado.net code */
            using (OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                List<Menu> menuList = new List<Menu>();
                OracleCommand cmd = new OracleCommand("GETROLEMENUDATA", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("rolenumber", rolenumber);
                cmd.Parameters.Add("menucur", OracleType.Cursor).Direction = System.Data.ParameterDirection.Output;

                con.Open();
                IDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    Menu menu = new Menu();
                    menu.MID = int.Parse(sdr["MID"].ToString());
                    menu.MenuName = sdr["MenuName"].ToString();
                    menu.MenuURL = sdr["MenuURL"].ToString();
                    menu.MenuIMG = sdr["MenuIMG"].ToString();
                    menu.MenuParentID = Convert.ToInt32(sdr["MenuParentID"].ToString());
                    //menu.subMenuParentID = Convert.ToInt32(sdr["MenuNameAR"].ToString());
                    menuList.Add(menu);
                }
                return menuList;
            }
        }


        public string GetActionStatus(string action_code)
        {
            string status = "N/A";

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select status from dispute_actions where action_code = '" + action_code + "'", con);
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    status = dr["status"].ToString();
                }
                else
                {
                    dr.Close();
                }
                dr.Close();
                con.Close();
            }
            return status;
        }


        public string GetBranchName(string branch_code)
        {
            string name = "N/A";

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select branch_name_en from branchs where branch_code = '" + branch_code + "'", con);
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    name = dr["branch_name_en"].ToString();
                }
                else
                {
                    dr.Close();
                }
                dr.Close();
                con.Close();
            }
            return name;
        }


        public int InsertActionComment(CustomerTransferReportViewModel model, string admin)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "INSERTCOMMENT";

                cmd.Parameters.Add("DISPUTE_ID", OracleType.VarChar).Value = model.dispute_id;
                cmd.Parameters.Add("STATUS", OracleType.VarChar).Value = model.TranStatus;
                cmd.Parameters.Add("COMMENTS", OracleType.VarChar).Value = model.Comment.ToString();
                cmd.Parameters.Add("USER_ENTRY", OracleType.VarChar).Value = admin;
                cmd.Parameters.Add("REASON_CODE", OracleType.VarChar).Value = model.REASON_CODE.ToString();
                cmd.Parameters.Add("ACTION_CODE", OracleType.VarChar).Value = model.selected_action.ToString();
                OracleParameter p = new OracleParameter("STATUSOUT", OracleType.VarChar, 2000);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);

                con.Open();
                int result = -1;
                result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        public int UpdateDispute(string id, string authorizor)
        {

            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("update disputes set status = 'U',authorizor = '" + authorizor + "' where id = '" + id + "'", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                return cmd.ExecuteNonQuery();
            }
        }

        internal string cpanel_editprofile(string profilename, string menuid, string parnetid, string profileid)
        {
            String lblconfirm = "System Error";
            bool FLAG;
            OracleCommand cmd;
            OracleDataReader dr;
            using (OracleConnection con = new OracleConnection(conString))
            {
                try
                {
                    cmd = new OracleCommand("select * from jsb_roles_master where role_name='" + profilename + "'", con);
                    OracleCommand cmd2;
                    OracleCommand cmd_acc_lnk;

                    con.Open();
                    cmd = new OracleCommand("select   max(to_number(nvl(mapping_id,0)))+ 1 maxid from jsb_role_menu_mapping", con);

                    String id = cmd.ExecuteScalar().ToString();
                    int newid = int.Parse(id);
                    newid = newid + 1;
                    cmd = new OracleCommand("select mapping_id, mapping_role_id ,mapping_menu_id ,mapping_status from jsb_role_menu_mapping where  mapping_menu_id ='" + parnetid + "' and mapping_role_id ='" + profileid + "'", con);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        cmd_acc_lnk = new OracleCommand("INSERT INTO jsb_role_menu_mapping (mapping_id, mapping_role_id ,mapping_menu_id ,mapping_status)VALUES('"
                           + newid + "','" + profileid + "','" + menuid + "','A')", con);
                        cmd_acc_lnk.ExecuteNonQuery();
                        lblconfirm = "Account Added Successfully";

                    }
                    else
                    {
                        cmd_acc_lnk = new OracleCommand("INSERT INTO jsb_role_menu_mapping (mapping_id, mapping_role_id ,mapping_menu_id ,mapping_status)VALUES('"
                            + newid + "','" + profileid + "','" + parnetid + "','A')", con);
                        cmd_acc_lnk.ExecuteNonQuery();
                        int nextid = newid + 1;
                        cmd_acc_lnk = new OracleCommand("INSERT INTO jsb_role_menu_mapping (mapping_id, mapping_role_id ,mapping_menu_id ,mapping_status)VALUES('"
                           + nextid + "','" + profileid + "','" + menuid + "','A')", con);
                        cmd_acc_lnk.ExecuteNonQuery();
                        lblconfirm = "Account Added Successfully";
                    }
                    con.Close();



                }
                catch (Exception ex)
                {
                    lblconfirm = "System Error";
                }
            }
            return lblconfirm;
        }

        public string GetCpanelprofilename(string roleid)
        {
            string profilename = "N/A";
            List<CustomerTransferReportViewModel> items = new List<CustomerTransferReportViewModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {

                string query = "select role_name from jsb_roles_master where role_id = '" + roleid + "'";

                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            profilename = dr["role_name"].ToString();
                        }
                        con.Close();
                    }
                }
            }

            return profilename;
        }

        public List<SelectListItem> PopulateCustStatus(string idotusername)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            char[] chararray = idotusername.ToCharArray();
            if (char.IsDigit(chararray[0]))
            {
                using (OracleConnection con = new OracleConnection(conString))
                {
                    string query = "select CUSTOMERSTATUS.STATUS_CODE,CUSTOMERSTATUS.STATUS_NAME from CUSTOMERSTATUS inner join users on CUSTOMERSTATUS.STATUS_CODE = users.USER_STATUS where CUSTOMERSTATUS.ACTIVE='1' and users.user_id = '" + idotusername + "' or users.user_log = '" + idotusername + "' or users.user_mobile = '" + idotusername + "' or  SUBSTR(users.def_acc,14,5) = '" + idotusername + "'";
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            if (sdr.HasRows)
                            {
                                //items.Add(new SelectListItem
                                //{
                                //    Text = "-- Select Customer Status --",
                                //    Value = "0",
                                //});
                                while (sdr.Read())
                                {
                                    items.Add(new SelectListItem
                                    {
                                        Text = sdr[1].ToString(),
                                        Value = sdr[0].ToString()
                                    });
                                }
                            }
                        }
                        con.Close();
                    }
                }
            }
            else
            {
                using (OracleConnection con = new OracleConnection(conString))
                {
                    string query = "select CUSTOMERSTATUS.STATUS_CODE,CUSTOMERSTATUS.STATUS_NAME from CUSTOMERSTATUS inner join users on CUSTOMERSTATUS.STATUS_CODE = users.USER_STATUS where CUSTOMERSTATUS.ACTIVE='1' and users.user_log = '" + idotusername + "' ";
                    using (OracleCommand cmd = new OracleCommand(query))
                    {
                        cmd.Connection = con;
                        con.Open();
                        using (OracleDataReader sdr = cmd.ExecuteReader())
                        {
                            if (sdr.HasRows)
                            {
                                //items.Add(new SelectListItem
                                //{
                                //    Text = "-- Select Customer Status --",
                                //    Value = "0",
                                //});
                                while (sdr.Read())
                                {
                                    items.Add(new SelectListItem
                                    {
                                        Text = sdr[1].ToString(),
                                        Value = sdr[0].ToString()
                                    });
                                }
                            }
                        }
                        con.Close();
                    }
                }
            }

            return items;
        }


        internal string addnewprofile(string profilename, string menuid, string parnetid)
        {
            String lblconfirm = "System Error", profileid = null;
            bool FLAG;
            OracleCommand cmd;
            OracleDataReader dr;
            using (OracleConnection con = new OracleConnection(conString))
            {
                try
                {
                    cmd = new OracleCommand("select * from tbl_rolemaster where name='" + profilename + "'", con);
                    OracleCommand cmd2;
                    OracleCommand cmd_acc_lnk;

                    con.Open();
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        profileid = dr[0].ToString();
                        FLAG = true;
                        lblconfirm = "These Account Already exist";

                    }
                    else
                    {

                        cmd = new OracleCommand("select max(to_number(nvl(roleid,0)))+1 from tbl_rolemaster", con);

                        profileid = cmd.ExecuteScalar().ToString();

                        cmd_acc_lnk = new OracleCommand(" INSERT INTO tbl_rolemaster (ROLEID,NAME,ACTIVE) VALUES ('"
                                            + profileid + "','" + profilename + "','1' )", con);
                        cmd_acc_lnk.ExecuteNonQuery();
                        lblconfirm = "Account Added Successfully";
                        FLAG = true;
                    }

                    if (FLAG == true)
                    {

                        cmd = new OracleCommand("select   max(to_number(nvl(id,0)))+ 1 maxid from tbl_rolemenumapping", con);

                        String id = cmd.ExecuteScalar().ToString();
                        cmd = new OracleCommand("select id,roleid,menuid,active from tbl_rolemenumapping where  menuid='" + parnetid + "' and roleid='" + profileid + "'", con);
                        dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            cmd_acc_lnk = new OracleCommand("INSERT INTO tbl_rolemenumapping (ID,ROLEID,MENUID,ACTIVE)VALUES('"
                               + id + "','" + profileid + "','" + menuid + "','1')", con);
                            cmd_acc_lnk.ExecuteNonQuery();
                            lblconfirm = "Account Added Successfully";

                        }
                        else
                        {
                            cmd_acc_lnk = new OracleCommand("INSERT INTO tbl_rolemenumapping (ID,ROLEID,MENUID,ACTIVE)VALUES('"
                                + id + "','" + profileid + "','" + parnetid + "','1')", con);
                            cmd_acc_lnk.ExecuteNonQuery();
                            cmd_acc_lnk = new OracleCommand("INSERT INTO tbl_rolemenumapping (ID,ROLEID,MENUID,ACTIVE)VALUES('"
                               + Convert.ToInt32(id) + 1 + "','" + profileid + "','" + menuid + "','1')", con);
                            cmd_acc_lnk.ExecuteNonQuery();
                            lblconfirm = "Account Added Successfully";
                        }
                    }
                    else
                    {
                        lblconfirm = "These Account Already exist";
                    }
                    con.Close();



                }
                catch (Exception ex)
                {
                    lblconfirm = "System Error";
                }
            }
            return lblconfirm;

        }

        internal string cpanel_addnewprofile(string profilename, string menuid, string parnetid , string usernmae)
        {
            String lblconfirm = "System Error", profileid = null;
            bool FLAG;
            OracleCommand cmd;
            OracleDataReader dr;

            
            using (OracleConnection con = new OracleConnection(conString))
            {
                try
                {
                    //cmd = new OracleCommand("select * from cpanel_rolemaster where name='" + profilename + "'", con);
                    cmd = new OracleCommand("select * from jsb_roles_master where role_name='" + profilename + "'", con);
                    OracleCommand cmd2;
                    OracleCommand cmd_acc_lnk;

                    con.Open();
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        profileid = dr[0].ToString();
                        FLAG = true;
                        lblconfirm = "These Account Already exist";

                    }
                    else
                    {
                        DateTime datetoday = DateTime.Now;
                        //cmd = new OracleCommand("select max(to_number(nvl(roleid,0)))+1 from cpanel_rolemaster", con);
                        cmd = new OracleCommand("select max(to_number(nvl(role_id,0)))+1 from jsb_roles_master", con);
                        profileid = cmd.ExecuteScalar().ToString();

                        //cmd_acc_lnk = new OracleCommand(" INSERT INTO cpanel_rolemaster (ROLEID,NAME,ACTIVE,INSERTED_DATE) VALUES ('"
                        //                    + profileid + "','" + profilename + "','1','" + datetoday.ToString() + "' )", con);
                        cmd_acc_lnk = new OracleCommand(" INSERT INTO jsb_roles_master (ROLE_ID,ROLE_NAME,ROLE_STATUS,ROLE_CREATION_DATE ,role_channel,role_created_by) VALUES ('"
                                           + profileid + "','" + profilename + "','A',sysdate ,'3' , '"+ usernmae + "')", con);

                        cmd_acc_lnk.ExecuteNonQuery();
                        lblconfirm = "Account Added Successfully";
                        FLAG = true;
                    }

                    if (FLAG == true)
                    {

                        //cmd = new OracleCommand("select   max(to_number(nvl(id,0)))+ 1 maxid from cpanel_rolemenumapping", con);
                        cmd = new OracleCommand("select   max(to_number(nvl(mapping_id,0)))+ 1 maxid from jsb_role_menu_mapping", con);
                        String id = cmd.ExecuteScalar().ToString();
                        cmd = new OracleCommand("select mapping_id,mapping_role_id,mapping_menu_id,mapping_status from jsb_role_menu_mapping where  mapping_menu_id='" + parnetid + "' and mapping_role_id='" + profileid + "'", con);
                        //cmd = new OracleCommand("select id,roleid,menuid,active from cpanel_rolemenumapping where  menuid='" + parnetid + "' and roleid='" + profileid + "'", con);

                        dr = cmd.ExecuteReader();
                        if (!dr.Read())
                        {
                            //cmd_acc_lnk = new OracleCommand("INSERT INTO cpanel_rolemenumapping (ID,ROLEID,MENUID,ACTIVE)VALUES('"
                            //   + id + "','" + profileid + "','" + menuid + "','1')", con);
                            cmd_acc_lnk = new OracleCommand("INSERT INTO jsb_role_menu_mapping (mapping_id,mapping_role_id,mapping_menu_id,mapping_status)VALUES('"
                              + id + "','" + profileid + "','" + menuid + "','A')", con);
                            cmd_acc_lnk.ExecuteNonQuery();
                            lblconfirm = "Account Added Successfully";

                        }
                        else
                        {
                            lblconfirm = "These Account Already exist";

                            //    cmd_acc_lnk = new OracleCommand("INSERT INTO jsb_role_menu_mapping (ID,ROLEID,MENUID,ACTIVE)VALUES('"
                            //        + id + "','" + profileid + "','" + parnetid + "','A')", con);
                            //    cmd_acc_lnk.ExecuteNonQuery();
                            //    cmd_acc_lnk = new OracleCommand("INSERT INTO jsb_role_menu_mapping (ID,ROLEID,MENUID,ACTIVE)VALUES('"
                            //       + Convert.ToInt32(id) + 1 + "','" + profileid + "','" + menuid + "','1')", con);
                            //    cmd_acc_lnk.ExecuteNonQuery();
                            //    lblconfirm = "Account Added Successfully";
                        }
                    }
                    else
                    {
                        lblconfirm = "These Account Already exist";
                    }
                    con.Close();



                }
                catch (Exception ex)
                {
                    lblconfirm = "System Error";
                }
            }
            return lblconfirm;

        }

        public List<channel> Channels()
        {
            List<channel> AvailableItems = new List<channel>();
            using (OracleConnection con = new OracleConnection(conString))
            {

                using (OracleCommand cmd = new OracleCommand())
                {

                    cmd.CommandText = "getchannel";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Connection = con;
                    con.Open();

                    cmd.Parameters.Add("channel_Cursor", OracleType.Cursor).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("res", OracleType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("errcode", OracleType.VarChar, 4000).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("errmsg", OracleType.VarChar, 4000).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();

                    String res = cmd.Parameters["res"].Value.ToString();
                    String errormsg = cmd.Parameters["errmsg"].Value.ToString();
                    String errorcode = cmd.Parameters["errcode"].Value.ToString();

                    using (OracleDataReader sdr = (OracleDataReader)cmd.Parameters["channel_Cursor"].Value)
                    {
                        while (sdr.Read())
                        {
                            AvailableItems.Add(new channel()
                            {
                                ID = sdr[0].ToString(),
                                Name = sdr[1].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return AvailableItems;
        }
        public List<UsersMangementViewModel> GetCustomerLog(String UserName, String loginType)
        {
            List<UsersMangementViewModel> userLog = new List<UsersMangementViewModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "";

                if (loginType.Equals("1"))
                {
                    query = "select ipaddress,login_time,user_login,user_pass,decode(login_status,'F','Failed','S','Succesful','unknown'), user_id from admin_login where user_login = '" + UserName + "'";

                }
                else if (loginType.Equals("2"))
                {
                    query = "select ipaddress,login_time,user_login,user_pass,decode(login_status,'F','Failed','S','Succesful','unknown'), user_id from admin_login where user_login = '" + UserName + "' and login_status = 'S'";

                }
                else
                {
                    query = "select ipaddress,login_time,user_login,user_pass,decode(login_status,'F','Failed','S','Succesful','unknown'), user_id from admin_login where user_login = '" + UserName + "' and login_status = 'F'";

                }

                //query = "select last_log_ip,last_login,user_log,decode(user_status,'A','Active','D','Deactive','DE','Deleted','U','Unauthorized','P','Pending'),decode(catogry,'1','Personal','2','Operator','3','Authorizor'), user_id from users";
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                OracleCommand cmd = new OracleCommand(query, con);

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {


                        userLog.Add(new UsersMangementViewModel
                        {
                            IpAddress = dr[0].ToString(),
                            LoginTime = dr[1].ToString(),
                            //user_id = Convert.ToInt32(dr[1].ToString()),
                            UserLogin = dr[2].ToString(),
                            UserPass = dr[3].ToString(),
                            LoginStatus = dr[4].ToString(),
                            UserID = dr[5].ToString(),

                        });
                    }
                }


            }
            return userLog;
        }

        public List<profilelist> GetRole()
        {
            List<profilelist> Roles = new List<profilelist>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "";


                //query = "select role_id , role_name,role_creation_date from jsb_roles_master";
                query = "select jsb_roles_master.role_id,jsb_roles_master.role_name,jsb_roles_master.role_creation_date,count(jsb_security_master.user_log)as usercount from jsb_roles_master left outer join jsb_security_master on jsb_roles_master.role_id = jsb_security_master.roleid group by jsb_roles_master.role_id,jsb_roles_master.role_name,jsb_roles_master.role_creation_date";


                //query = "select last_log_ip,last_login,user_log,decode(user_status,'A','Active','D','Deactive','DE','Deleted','U','Unauthorized','P','Pending'),decode(catogry,'1','Personal','2','Operator','3','Authorizor'), user_id from users";
                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                OracleCommand cmd = new OracleCommand(query, con);

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {


                        Roles.Add(new profilelist
                        {
                            role_id = int.Parse( dr[0].ToString()),
                            name = dr[1].ToString(),
                            //user_id = Convert.ToInt32(dr[1].ToString()),
                            inserted_date = dr[2].ToString(),
                            users_count = dr[3].ToString()
                            //LoginStatus = dr[4].ToString(),
                            //UserID = dr[5].ToString(),

                        });
                    }
                }


            }
            return Roles;
        }

        public List<UsersMangementViewModel> GetCustomersLog()
        {
            List<UsersMangementViewModel> userLog = new List<UsersMangementViewModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select user_log,last_login,decode(user_status,'A','Active','D','Deactive','DE','Deleted','U','Unauthorized','P','Pending','N/A'),decode(catogry,'1','Personal','2','Operator','3','Authorizor','N/A'),last_log_ip,user_id from users order by to_date(last_login) desc";
                if (con.State == ConnectionState.Closed)
                { con.Open(); }
                OracleCommand cmd = new OracleCommand(query, con);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        userLog.Add(new UsersMangementViewModel
                        {
                            Username = dr[0].ToString(),
                            LoginTime = dr[1].ToString(),
                            IpAddress = dr[4].ToString(),
                            UserStatus = dr[2].ToString(),
                            Category = dr[3].ToString(),
                            UserID = dr[5].ToString(),
                        });
                    }
                }
            }
            return userLog;
        }

        public List<UsersMangementViewModel> GetCustomersLog(string fromdate, string todate)
        {
            List<UsersMangementViewModel> userLog = new List<UsersMangementViewModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select user_log,last_login,decode(user_status,'A','Active','D','Deactive','DE','Deleted','U','Unauthorized','P','Pending','N/A'),decode(catogry,'1','Personal','2','Operator','3','Authorizor','N/A'),last_log_ip,user_id from users   and to_date(substr(created_date,0,9),'dd-mon-yy') >= to_date('" + fromdate + "','mm-dd-yyyy') and to_date(substr(created_date,0,9),'dd-mon-yy') <= to_date('" + todate + "','mm-dd-yyyy')";
                if (con.State == ConnectionState.Closed)
                { con.Open(); }
                OracleCommand cmd = new OracleCommand(query, con);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        userLog.Add(new UsersMangementViewModel
                        {
                            Username = dr[0].ToString(),
                            LoginTime = dr[1].ToString(),
                            IpAddress = dr[4].ToString(),
                            UserStatus = dr[2].ToString(),
                            Category = dr[3].ToString(),
                            UserID = dr[5].ToString(),
                        });
                    }
                }
            }
            return userLog;
        }

        public List<SelectListItem> billers_statuses()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select bil_name ,bil_billerid from billers_statuses";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {

                            items.Add(new SelectListItem
                            {
                                Text = " Select Billers ",
                                Value = "0",
                            });

                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    //Text = sdr["bil_name"].ToString(),
                                    //Value = sdr["bil_billerid"].ToString()

                                    Text = sdr["bil_name"].ToString(),
                                    Value = sdr["bil_billerid"].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }


            return items;
        }

        internal int insertuser(string p1, string p2, string p3, string p4)
        {
            throw new NotImplementedException();
        }
        //--------------------------GET UserLog------------
        public List<UsersMangementViewModel> GetUserLog(String UserName, String loginType)
        {
            List<UsersMangementViewModel> userLog = new List<UsersMangementViewModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "";

                if (loginType.Equals("1"))
                {
                    query = "select ipaddress,login_time,user_login,user_pass,decode(login_status,'F','Failed','S','Succesful','unknown') from users_login where user_login = '" + UserName + "'";

                }
                else if (loginType.Equals("2"))
                {
                    query = "select ipaddress,login_time,user_login,user_pass,decode(login_status,'F','Failed','S','Succesful','unknown') from users_login where user_login = '" + UserName + "' and login_status = 'S'";

                }
                else
                {
                    query = "select ipaddress,login_time,user_login,user_pass,decode(login_status,'F','Failed','S','Succesful','unknown') from users_login where user_login = '" + UserName + "' and login_status = 'F'";

                }

                if (con.State == ConnectionState.Closed)
                { con.Open(); }

                OracleCommand cmd = new OracleCommand(query, con);

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        userLog.Add(new UsersMangementViewModel
                        {
                            IpAddress = dr[0].ToString(),
                            LoginTime = dr[1].ToString(),
                            //user_id = Convert.ToInt32(dr[1].ToString()),
                            UserLogin = dr[2].ToString(),
                            UserPass = dr[3].ToString(),
                            LoginStatus = dr[4].ToString(),

                        });
                    }

                }


            }
            return userLog;
        }

        //---------------------------------GetCustomerIDFromAccountNumber------------------------------------------
        /// <summary>
        /// /It Gets Customer Full Account Number
        /// and Returns the ID
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <returns>CustID</returns>
        /// 
        public String getCustIDFromAcc(string AccountNumber)
        {
            int CustID = 0;
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select user_id from users_jsb where def_acc = " + AccountNumber;

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();


                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {

                        if (dataReader["user_id"] != DBNull.Value)
                        {

                            CustID = Convert.ToInt32(dataReader["user_id"]);

                        }
                    }
                    //Accounts = Accounts.Substring(1);
                    return CustID.ToString();

                }

            }

        }


        public String getCustNoFromRim(string AccountNumber)
        {
            string AccNo = "";
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select def_acc from users_jsb where user_rim = :accountNumber", con);
                cmd.Parameters.Add("accountNumber", OracleType.VarChar).Value = AccountNumber;

                con.Open();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        if (dataReader["def_acc"] != DBNull.Value)
                        {
                            AccNo = dataReader["def_acc"].ToString();
                        }
                    }
                    return AccNo;
                }
            }
        }


        public List<CustomerRegBankinfo> getCustNoFromRimCorp(string AccountNumber, string cat)
        {
            List<CustomerRegBankinfo> info = new List<CustomerRegBankinfo>();

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select def_acc, user_log from users_jsb where user_rim = :accountNumber";
                if (cat == "2")
                {
                    query += " and user_log like '%O%'";
                }
                else if (cat == "3")
                {
                    query += " and user_log like '%A%'";
                }

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("accountNumber", OracleType.VarChar).Value = AccountNumber;

                con.Open();
                OracleDataReader dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        info.Add(new CustomerRegBankinfo
                        {
                            AccountNumberAdded = dataReader[0].ToString(),
                            CustomerID = dataReader[1].ToString(),
                        });
                    }
                }
                return info;

            }

        }


        //----------------------------------------------GetTransferReport---------------------------------------------------------
        /// <summary>
        /// GetTransferReport
        /// </summary>
        /// <param name="custId"></param>
        /// <returns>List of Requests and response</returns>
        public List<CustomerTransferReportViewModel> GetTransferReport(string custId)
        {
            List<CustomerTransferReportViewModel> items = new List<CustomerTransferReportViewModel>();
            using (OracleConnection con = new OracleConnection(conString))
            {

                string query = " SELECT tran_req_date,tran_req,tran_resp,tran_resp_result,TRAN_STATUS from trans_log WHERE" +
                               " tran_name in('Own Transfer','To Bank Customer Transfer','To Counter Transfer') AND user_id = " + custId;

                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                       String amount, accountFrom, accountTo, accountFromBranch, accountFromAccType, fromAcc_Info; 
                        while (dr.Read())
                        {
                            dynamic jsonRequesrt = JsonConvert.DeserializeObject(dr["tran_req"].ToString());
                            amount = jsonRequesrt["amount"];
                            accountFrom = jsonRequesrt["accountfrom"];
                            fromAcc_Info = getFromAccInfo(accountFrom);
                           // accountFromBranch = getbranchnameenglish(accountFrom.Substring(2, 3));
                            //accountFromAccType = getaccounttype(accountFrom.Substring(5, 5));
                            //accountFromNew = "Branch:  " + accountFromBranch + " " + "  Account Type:  " + accountFromAccType;
                            accountTo = jsonRequesrt["accountto"];
                            string[] reqString = null;
                            items.Add(new CustomerTransferReportViewModel
                            {
                                TranDate = dr["tran_req_date"].ToString(),
                                //TranFullReq = dr["tran_req"].ToString(),
                                TranAmount = amount.ToString(),
                                //TranFromAccount = accountFrom.ToString(),
                                TranFromAccount = fromAcc_Info.ToString(),
                                TranToAccount = accountTo.ToString(),
                                TranFullResp = dr["tran_resp"].ToString(),
                                TranResult = dr["tran_resp_result"].ToString(),
                                TranStatus = dr["TRAN_STATUS"].ToString(),

                            });
                        }
                        con.Close();
                    }
                }
            }

            return items;
        }

        public List<SelectListItem> PopulateBranchs(string branchcode, string idorusername)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //char[] chararray = idorusername.ToCharArray();
            //if (char.IsDigit(chararray[0]) && idorusername.Length == 12)
            //{
            //    using (OracleConnection con = new OracleConnection(conString))
            //    {
            //        string query = " select distinct branchs.branch_code,branchs.branch_name from branchs left outer join users on branchs.branch_code = SUBSTR(users.def_acc,3,3) where branchs.branch_sts = '1' and branchs.BRANCH_CODE_NO ='" + branchcode + "' and user_log = '" + idorusername + "'";
            //        using (OracleCommand cmd = new OracleCommand(query))
            //        {
            //            cmd.Connection = con;
            //            con.Open();
            //            using (OracleDataReader sdr = cmd.ExecuteReader())
            //            {
            //                if (sdr.HasRows)
            //                {
            //                    while (sdr.Read())
            //                    {
            //                        items.Add(new SelectListItem
            //                        {
            //                            Text = sdr["branch_name"].ToString(),
            //                            Value = sdr["branch_code"].ToString()
            //                        });
            //                    }
            //                }
            //            }
            //            con.Close();
            //        }
            //    }
            //}
            //else if (char.IsDigit(chararray[0]))
            //{
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select distinct branchs.branch_code,branchs.branch_name_en from branchs left outer join users_jsb on branchs.branch_code = SUBSTR(users_jsb.def_acc,3,3) where branchs.branch_status = 'A' and branchs.BRANCH_CODE ='" + branchcode + "' and user_log = '" + idorusername + "' or user_mobile = '" + idorusername + "' or SUBSTR(users_jsb.def_acc,14,5) = '" + idorusername + "'";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["branch_name_en"].ToString(),
                                    Value = sdr["branch_code"].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }
            //}
            //else
            //{
            //    using (OracleConnection con = new OracleConnection(conString))
            //    {
            //        string query = " select distinct branchs.branch_code,branchs.branch_name from branchs left outer join users on branchs.branch_code = SUBSTR(users.def_acc,3,3) where branchs.branch_sts = '1' and branchs.BRANCH_CODE_NO ='" + branchcode + "' and user_log = '" + idorusername + "'";
            //        using (OracleCommand cmd = new OracleCommand(query))
            //        {
            //            cmd.Connection = con;
            //            con.Open();
            //            using (OracleDataReader sdr = cmd.ExecuteReader())
            //            {
            //                if (sdr.HasRows)
            //                {
            //                    while (sdr.Read())
            //                    {
            //                        items.Add(new SelectListItem
            //                        {
            //                            Text = sdr["branch_name"].ToString(),
            //                            Value = sdr["branch_code"].ToString()
            //                        });
            //                    }
            //                }
            //            }
            //            con.Close();
            //        }
            //    }
            //}

            return items;
        }

        public List<SelectListItem> PopulateAccountTypes(string idorusername)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //char[] chararray = idorusername.ToCharArray();
            //if (char.IsDigit(chararray[0]) && idorusername.Length == 12)
            //{
            using (OracleConnection con = new OracleConnection(conString))
            {
                //string query = "select act_type_code,act_name_en from Act_types inner join users_jsb on act_type_code = SUBSTR(def_acc,6,5) and user_log = '" + idorusername + "' ";
                string query = "select act_type_code,act_name_en from Act_types where  act_type_code = '" + idorusername + "' ";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add( new SelectListItem
                            {
                                Text = sdr["act_name_en"].ToString(),
                                Value = sdr["act_type_code"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            //}
            //else if (char.IsDigit(chararray[0]))
            //{
            //    using (OracleConnection con = new OracleConnection(conString))
            //    {
            //        string query = "select act_type_code,act_name from Act_types inner join users on act_type_code = SUBSTR(def_acc,6,6) and user_id = '" + Convert.ToInt64(idorusername) + "'";
            //        using (OracleCommand cmd = new OracleCommand(query))
            //        {
            //            cmd.Connection = con;
            //            con.Open();
            //            using (OracleDataReader sdr = cmd.ExecuteReader())
            //            {
            //                while (sdr.Read())
            //                {
            //                    items.Insert(0, new SelectListItem
            //                    {
            //                        Text = sdr["act_name"].ToString(),
            //                        Value = sdr["act_type_code"].ToString(),
            //                    });
            //                }
            //            }
            //            con.Close();
            //        }
            //    }
            //}
            //else
            //{
            //    using (OracleConnection con = new OracleConnection(conString))
            //    {
            //        string query = "select act_type_code,act_name from Act_types inner join users on act_type_code = SUBSTR(def_acc,6,6) and user_log = '" + idorusername + "'";
            //        using (OracleCommand cmd = new OracleCommand(query))
            //        {
            //            cmd.Connection = con;
            //            con.Open();
            //            using (OracleDataReader sdr = cmd.ExecuteReader())
            //            {
            //                while (sdr.Read())
            //                {
            //                    items.Insert(0, new SelectListItem
            //                    {
            //                        Text = sdr["act_name"].ToString(),
            //                        Value = sdr["act_type_code"].ToString(),
            //                    });
            //                }
            //            }
            //            con.Close();
            //        }
            //    }
            //}



            return items;
        }



        public List<SelectListItem> PopulateBranchslink( string idorusername)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //char[] chararray = idorusername.ToCharArray();
            //if (char.IsDigit(chararray[0]) && idorusername.Length == 12)
            //{
            //    using (OracleConnection con = new OracleConnection(conString))
            //    {
            //        string query = " select distinct branchs.branch_code,branchs.branch_name from branchs left outer join users on branchs.branch_code = SUBSTR(users.def_acc,3,3) where branchs.branch_sts = '1' and branchs.BRANCH_CODE_NO ='" + branchcode + "' and user_log = '" + idorusername + "'";
            //        using (OracleCommand cmd = new OracleCommand(query))
            //        {
            //            cmd.Connection = con;
            //            con.Open();
            //            using (OracleDataReader sdr = cmd.ExecuteReader())
            //            {
            //                if (sdr.HasRows)
            //                {
            //                    while (sdr.Read())
            //                    {
            //                        items.Add(new SelectListItem
            //                        {
            //                            Text = sdr["branch_name"].ToString(),
            //                            Value = sdr["branch_code"].ToString()
            //                        });
            //                    }
            //                }
            //            }
            //            con.Close();
            //        }
            //    }
            //}
            //else if (char.IsDigit(chararray[0]))
            //{
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select distinct branchs.branch_code,branchs.branch_name_en from branchs left outer join user_acc_link_jsb on branchs.branch_code = user_acc_link_jsb.acc_branch where branchs.branch_status = 'A'  and user_acc_link_jsb.acc_no = '" + idorusername + "'";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["branch_name_en"].ToString(),
                                    Value = sdr["branch_code"].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }
            //}
            //else
            //{
            //    using (OracleConnection con = new OracleConnection(conString))
            //    {
            //        string query = " select distinct branchs.branch_code,branchs.branch_name from branchs left outer join users on branchs.branch_code = SUBSTR(users.def_acc,3,3) where branchs.branch_sts = '1' and branchs.BRANCH_CODE_NO ='" + branchcode + "' and user_log = '" + idorusername + "'";
            //        using (OracleCommand cmd = new OracleCommand(query))
            //        {
            //            cmd.Connection = con;
            //            con.Open();
            //            using (OracleDataReader sdr = cmd.ExecuteReader())
            //            {
            //                if (sdr.HasRows)
            //                {
            //                    while (sdr.Read())
            //                    {
            //                        items.Add(new SelectListItem
            //                        {
            //                            Text = sdr["branch_name"].ToString(),
            //                            Value = sdr["branch_code"].ToString()
            //                        });
            //                    }
            //                }
            //            }
            //            con.Close();
            //        }
            //    }
            //}

            return items;
        }

        public List<SelectListItem> PopulateAccountTypeslink(string idorusername)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //char[] chararray = idorusername.ToCharArray();
            //if (char.IsDigit(chararray[0]) && idorusername.Length == 12)
            //{
            using (OracleConnection con = new OracleConnection(conString))
            {
                //string query = "select act_type_code,act_name_en from Act_types inner join users_jsb on act_type_code = SUBSTR(def_acc,6,5) and user_log = '" + idorusername + "' ";
                string query = "select act_type_code,act_name_en from act_types left outer join  user_acc_link_jsb on act_types.act_type_code  = user_acc_link_jsb.acc_type where  user_acc_link_jsb.acc_no = '" + idorusername + "' ";
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read()) 
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["act_name_en"].ToString(),
                                Value = sdr["act_type_code"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            //}
            //else if (char.IsDigit(chararray[0]))
            //{
            //    using (OracleConnection con = new OracleConnection(conString))
            //    {
            //        string query = "select act_type_code,act_name from Act_types inner join users on act_type_code = SUBSTR(def_acc,6,6) and user_id = '" + Convert.ToInt64(idorusername) + "'";
            //        using (OracleCommand cmd = new OracleCommand(query))
            //        {
            //            cmd.Connection = con;
            //            con.Open();
            //            using (OracleDataReader sdr = cmd.ExecuteReader())
            //            {
            //                while (sdr.Read())
            //                {
            //                    items.Insert(0, new SelectListItem
            //                    {
            //                        Text = sdr["act_name"].ToString(),
            //                        Value = sdr["act_type_code"].ToString(),
            //                    });
            //                }
            //            }
            //            con.Close();
            //        }
            //    }
            //}
            //else
            //{
            //    using (OracleConnection con = new OracleConnection(conString))
            //    {
            //        string query = "select act_type_code,act_name from Act_types inner join users on act_type_code = SUBSTR(def_acc,6,6) and user_log = '" + idorusername + "'";
            //        using (OracleCommand cmd = new OracleCommand(query))
            //        {
            //            cmd.Connection = con;
            //            con.Open();
            //            using (OracleDataReader sdr = cmd.ExecuteReader())
            //            {
            //                while (sdr.Read())
            //                {
            //                    items.Insert(0, new SelectListItem
            //                    {
            //                        Text = sdr["act_name"].ToString(),
            //                        Value = sdr["act_type_code"].ToString(),
            //                    });
            //                }
            //            }
            //            con.Close();
            //        }
            //    }
            //}



            return items;
        }

        public CustomerRegBankinfo GetUserRegistrationDatalink(string idorname , string cat)
        {
            CustomerRegBankinfo usermodel = new CustomerRegBankinfo();
            if (idorname != null)
            {
                //char[] chararray = idorname.ToCharArray();
                //if (char.IsDigit(chararray[0]) && idorname.Length == 12)
                //{
                using (OracleConnection con = new OracleConnection(conString))
                {
                    string query = "select user_name_en,def_acc as account_number , roleid ,user_profile_id ,user_log from users_jsb where user_rim = '" + idorname + "'  and roleid = '"+cat+"' ";

                    OracleCommand cmd = new OracleCommand(query, con);
                    con.Open();
                    using (IDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {


                            //usermodel.BranchCode = dataReader["branch_code"].ToString();
                            //usermodel.Branch = dataReader["branch_name"].ToString();
                            //usermodel.CurrencyCode = dataReader["currency_code"].ToString();
                            //usermodel.Currency = dataReader["currency_name"].ToString();
                            //usermodel.AccountTypecode = dataReader["account_type_code"].ToString();
                            usermodel.AccountType = dataReader["user_profile_id"].ToString();
                            usermodel.AccountNumber = dataReader["account_number"].ToString();
                            usermodel.CategoryCode = dataReader["roleid"].ToString();
                            //usermodel.category = dataReader["category_name"].ToString();
                            usermodel.CustomerName = dataReader["user_name_en"].ToString();
                            usermodel.CustomerID = dataReader["user_log"].ToString(); 


                        }
                    }
                    return usermodel;
                }

            }
            return usermodel;
        }


        public CustomerRegBankinfo GetUserRegistrationData(string idorname)
        {
            CustomerRegBankinfo usermodel = new CustomerRegBankinfo();
            if (idorname != null)
            {
                using (OracleConnection con = new OracleConnection(conString))
                {
                    // WAPT01-02: Parameterized — no string concatenation
                    OracleCommand cmd = new OracleCommand(
                        "SELECT user_name_en, def_acc AS account_number, user_type, user_status " +
                        "FROM users_jsb WHERE user_log = :idorname", con);
                    cmd.Parameters.Add("idorname", OracleType.VarChar).Value = idorname;

                    con.Open();
                    using (IDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            //usermodel.BranchCode = dataReader["branch_code"].ToString();
                            //usermodel.Branch = dataReader["branch_name"].ToString();
                            //usermodel.CurrencyCode = dataReader["currency_code"].ToString();
                            //usermodel.Currency = dataReader["currency_name"].ToString();
                            //usermodel.AccountTypecode = dataReader["account_type_code"].ToString();
                            usermodel.AccountType = dataReader["user_type"].ToString();
                            usermodel.AccountNumber = dataReader["account_number"].ToString();
                            usermodel.status = dataReader["user_status"].ToString();
                            //usermodel.category = dataReader["category_name"].ToString();
                            usermodel.CustomerName = dataReader["user_name_en"].ToString();
                            usermodel.CustomerID = idorname;
                        }
                    }
                    return usermodel;
                }
                //}
                //else if (char.IsDigit(chararray[0]))
                //{
                //    using (OracleConnection con = new OracleConnection(conString))
                //    {
                //        string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_name, SUBSTR(def_acc,6,6) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,6)) as account_type,SUBSTR(def_acc,12,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name,SUBSTR(def_acc,19,2) as subno, SUBSTR(def_acc,23,3) as subgl from users where user_id = '" + idorname + "'";

                //        OracleCommand cmd = new OracleCommand(query, con);
                //        con.Open();
                //        using (IDataReader dataReader = cmd.ExecuteReader())
                //        {
                //            while (dataReader.Read())
                //            {
                //                usermodel.BranchCode = dataReader["branch_code"].ToString();
                //                usermodel.Branch = dataReader["branch_name"].ToString();
                //                usermodel.CurrencyCode = dataReader["currency_code"].ToString();
                //                usermodel.Currency = dataReader["currency_name"].ToString();
                //                usermodel.AccountTypecode = dataReader["account_type_code"].ToString();
                //                usermodel.AccountType = dataReader["account_type"].ToString();
                //                usermodel.AccountNumber = dataReader["account_number"].ToString();
                //                usermodel.CategoryCode = dataReader["category_id"].ToString();
                //                usermodel.category = dataReader["category_name"].ToString();
                //                usermodel.SUBNO = dataReader["subno"].ToString();
                //                usermodel.SUBGL = dataReader["subgl"].ToString();
                //                usermodel.CustomerID = idorname;
                //            }
                //        }
                //        return usermodel;
                //    }
                //}
                //else
                //{
                //    using (OracleConnection con = new OracleConnection(conString))
                //    {
                //        //string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_name, SUBSTR(def_acc,6,6) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,6)) as account_type,SUBSTR(def_acc,12,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name from users where user_log = '" + idorname + "'";
                //        string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_name, SUBSTR(def_acc,6,6) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,6)) as account_type,SUBSTR(def_acc,12,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name,SUBSTR(def_acc,19,2) as subno, SUBSTR(def_acc,23,3) as subgl from users where user_log = '" + idorname + "'";
                //        OracleCommand cmd = new OracleCommand(query, con);
                //        con.Open();
                //        using (IDataReader dataReader = cmd.ExecuteReader())
                //        {
                //            while (dataReader.Read())
                //            {
                //                usermodel.BranchCode = dataReader["branch_code"].ToString();
                //                usermodel.Branch = dataReader["branch_name"].ToString();
                //                usermodel.CurrencyCode = dataReader["currency_code"].ToString();
                //                usermodel.Currency = dataReader["currency_name"].ToString();
                //                usermodel.AccountTypecode = dataReader["account_type_code"].ToString();
                //                usermodel.AccountType = dataReader["account_type"].ToString();
                //                usermodel.AccountNumber = dataReader["account_number"].ToString();
                //                usermodel.CategoryCode = dataReader["category_id"].ToString();
                //                usermodel.category = dataReader["category_name"].ToString();
                //                usermodel.SUBNO = dataReader["subno"].ToString();
                //                usermodel.SUBGL = dataReader["subgl"].ToString();
                //                usermodel.CustomerID = idorname;
                //            }
                //        }
                //        return usermodel;
                //    }
                //}
            }
            return usermodel;
        }

        public CustomerTransferReportViewModel GetUserReportData(string idorname)
        {
            CustomerTransferReportViewModel usermodel = new CustomerTransferReportViewModel();
            char[] chararray = idorname.ToCharArray();
            if (char.IsDigit(chararray[0]))
            {
                using (OracleConnection con = new OracleConnection(conString))
                {
                    //string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_name, SUBSTR(def_acc,6,5) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,5)) as account_type,SUBSTR(def_acc,11,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name from users where user_id = '" + int.Parse(idorname) + "'";
                    string query = "select user_name, def_acc as account_number from users where user_id = '" + idorname + "' or user_mobile = '" + idorname + "' or  SUBSTR(users.def_acc,14,7) = '" + idorname + "' or user_log = '" + idorname + "'";
                    OracleCommand cmd = new OracleCommand(query, con);
                    con.Open();
                    using (IDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            //usermodel.BranchCode = dataReader["branch_code"].ToString();
                            //usermodel.Branch = dataReader["branch_name"].ToString();
                            //usermodel.CurrencyCode = dataReader["currency_code"].ToString();
                            //usermodel.Currency = dataReader["currency_name"].ToString();
                            //usermodel.AccountTypecode = dataReader["account_type_code"].ToString();
                            //usermodel.AccountType = dataReader["account_type"].ToString();
                            usermodel.AccountNumber = dataReader["account_number"].ToString();
                            usermodel.CustomerName = dataReader["user_name"].ToString();
                            usermodel.CustomerID = idorname;
                        }
                    }
                    return usermodel;
                }
            }
            else
            {
                using (OracleConnection con = new OracleConnection(conString))
                {
                    string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,21,2)) as currency_name, SUBSTR(def_acc,6,6) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,6)) as account_type,SUBSTR(def_acc,12,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name,SUBSTR(def_acc,19,2) as subno, SUBSTR(def_acc,23,3) as subgl from users where user_log = '" + idorname + "'";
                    //string query = "select (select branch_code from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_code, (select branch_name from branchs where branch_code = SUBSTR(users.def_acc,3,3)) as branch_name, (select curr_code from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_code, (select curr_name from currency where curr_code =  SUBSTR(def_acc,11,3)) as currency_name, SUBSTR(def_acc,6,5) as account_type_code,(select act_name from act_types where ACT_TYPE_CODE = SUBSTR(def_acc,6,5)) as account_type,SUBSTR(def_acc,11,7) as account_number, (select cat_id from category where cat_id = users.catogry) as category_id,(select cat_name from category where cat_id = users.catogry) as category_name from users where user_log = '" + idorname + "'";
                    OracleCommand cmd = new OracleCommand(query, con);
                    con.Open();
                    using (IDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            usermodel.BranchCode = dataReader["branch_code"].ToString();
                            usermodel.Branch = dataReader["branch_name"].ToString();
                            usermodel.CurrencyCode = dataReader["currency_code"].ToString();
                            usermodel.Currency = dataReader["currency_name"].ToString();
                            usermodel.AccountTypecode = dataReader["account_type_code"].ToString();
                            usermodel.AccountType = dataReader["account_type"].ToString();
                            usermodel.AccountNumber = dataReader["account_number"].ToString();
                            usermodel.SUBNO = dataReader["SUBNO"].ToString();
                            usermodel.SUBGL = dataReader["SUBGL"].ToString();
                            usermodel.CustomerID = idorname;
                        }
                    }
                    return usermodel;
                }
            }
        }



        public List<Servielist> GetAllServices()
        {
            List<Servielist> service = new List<Servielist>();
            using (OracleConnection con = new OracleConnection(conString))
            {
                OracleCommand cmd = new OracleCommand("select service_id,service_name,service_code,decode(service_status,'A','Active','DE','Delete','Unknown') from SERVICES where service_status='A' ", con);
                if (con.State == ConnectionState.Closed)
                { con.Open(); }


                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {


                        service.Add(new Servielist
                        {
                            service_id = Convert.ToInt32(dr[0].ToString()),

                            name = dr[1].ToString(),
                            service_code = dr[2].ToString(),
                            service_status = dr[3].ToString()
                        });
                    }
                }


            }
            return service;
        }

       

        public List<ActionsLogViewModel> getactionslog()
        {
            List<ActionsLogViewModel> actions = new List<ActionsLogViewModel>();
            //String sqlinc = "";
            //int offset = pageNumber * 500;
            //sqlinc = " OFFSET " + offset + "  ROWS FETCH NEXT 500 ROWS ONLY";

            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select * from admins_log  "; //update for all not  //where action = 'Customer information inquiry'
                OracleCommand cmd = new OracleCommand(query, con);
                con.Open();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        ActionsLogViewModel action = new ActionsLogViewModel();
                        action.user_id = dataReader["USER_ID"].ToString();
                        action.user_name = dataReader["USERNAME"].ToString();
                        action.user_role = dataReader["USER_ROLE"].ToString();
                        action.user_status = dataReader["USER_STATUS"].ToString();
                        action.action = dataReader["ACTION"].ToString();
                        action.action_on_user = dataReader["ACTION_ON_USER"].ToString();
                        action.timedate = dataReader["TIMEDATE"].ToString();
                        action.user_branch = dataReader["USER_BRANCH"].ToString();

                        actions.Add(action);
                    }
                }
            }
            return actions;
        }

        //-----------------------------------getTransactions---------------------
        /// <summary>
        /// Get top 5 Transactions
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public List<LatestTransactions> getTransactions(string user_id)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                //string query = "select tran_id,tran_name,tran_status,tran_resp_result from trans_log where user_id =" + user_id + "and ROWNUM <= 5 order by rownum desc";
                string query =
                    "select * from ( select tran_id , tran_name,tran_status,tran_resp_result from trans_log where user_id = '" +
                    user_id + "' order by tran_id desc ) where rownum <= 5";


                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();

                List<LatestTransactions> list = new List<LatestTransactions>();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        LatestTransactions obj = new LatestTransactions();

                        if (dataReader["tran_id"] != DBNull.Value)
                        {
                            //obj.AccountID = (int)dataReader["acc_id"];
                            obj.TranId = Convert.ToInt32(dataReader["tran_id"]);
                        }
                        if (dataReader["tran_name"] != DBNull.Value)
                        {
                            obj.TranName = (string)dataReader["tran_name"];
                        }
                        if (dataReader["tran_status"] != DBNull.Value)
                        {
                            obj.TranStatus = (string)dataReader["tran_status"];
                        }
                        if (dataReader["tran_resp_result"] != DBNull.Value)
                        {
                            obj.TranResult = (string)dataReader["tran_resp_result"];
                        }
                        list.Add(obj);
                    }

                    return list;

                }

            }

        }



        //-----------------------GET Transfer Count------------------------------------------------------
        //
        public String GetTransferCount(string user_id)
        {
            String count = "NULL";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select count(*) from trans_log where    (tran_name = 'Own Transfer' or tran_name = 'To Bank Customer Transfer')";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();


                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {



                        if (dataReader["count(*)"] != DBNull.Value)
                        {
                            count = dataReader["count(*)"].ToString();
                        }

                    }
                }

                return count;

            }
        }

        public String GetFailedCount(string user_id)
        {
            String count = "NULL";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select count(*) from trans_log where tran_status like '%Failed%' ";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();


                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {



                        if (dataReader["count(*)"] != DBNull.Value)
                        {
                            count = dataReader["count(*)"].ToString();
                        }

                    }
                }

                return count;

            }
        }
        public String GetSecussfullyCount(string user_id)
        {
            String count = "NULL";
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = " select count(*) from trans_log where tran_status like '%Secussfully%' ";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();


                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {



                        if (dataReader["count(*)"] != DBNull.Value)
                        {
                            count = dataReader["count(*)"].ToString();
                        }

                    }
                }

                return count;

            }
        }

        //-----------------------GET Accounts Count------------------------------------------------------
        //
        public String GetAccountsCount(string branchcode)
        {
            String count = "NULL", query;
            using (OracleConnection con = new OracleConnection(conString))
            {
                if (!branchcode.Equals("000"))
                {
                    query = " SELECT count(*) from user_acc_link_jsb where branch_code = " + branchcode;
                }
                else
                {
                    query = " SELECT count(*) from user_acc_link_jsb ";
                }
                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();


                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {



                        if (dataReader["count(*)"] != DBNull.Value)
                        {
                            count = dataReader["count(*)"].ToString();
                        }

                    }
                }

                return count;

            }
        }

        public String GetUsersCount(string branchcode)
        {
            String count = "NULL", query;
            using (OracleConnection con = new OracleConnection(conString))
            {
                if (!branchcode.Equals("000"))
                {
                    query = " SELECT count(*) from users where substr(def_acc,3,3)= " + branchcode;
                }
                else
                {
                    query = " SELECT count(*) from users ";
                }
                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();


                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {



                        if (dataReader["count(*)"] != DBNull.Value)
                        {
                            count = dataReader["count(*)"].ToString();
                        }

                    }
                }

                return count;

            }
        }

        public UserDetailsModel GetUserDetails(string IdOrName)
        {
            UserDetailsModel usermodel = new UserDetailsModel();
            using (OracleConnection con = new OracleConnection(conString))
            {
                string query = "select * from users where user_log = :IdOrName";
                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add("IdOrName", OracleType.VarChar).Value = IdOrName;
                con.Open();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        usermodel.user_id = int.Parse(dataReader["user_id"].ToString());
                        usermodel.user_name = dataReader["user_name"].ToString();
                        usermodel.user_log = dataReader["user_log"].ToString();
                        usermodel.user_email = dataReader["user_email"].ToString();
                        usermodel.user_mobile = dataReader["user_mobile"].ToString();
                        usermodel.user_fax = dataReader["user_fax"].ToString();
                        usermodel.user_address = dataReader["user_adrs"].ToString();
                        usermodel.user_status = dataReader["user_status"].ToString();
                        usermodel.defult_account = dataReader["def_acc"].ToString();
                        usermodel.last_login = dataReader["last_login"].ToString();
                        usermodel.last_login_ip = dataReader["last_log_ip"].ToString();
                        usermodel.faild_login = int.Parse(dataReader["faild_logins"].ToString());
                        usermodel.first_login = dataReader["first_login"].ToString();
                        usermodel.category = int.Parse(dataReader["catogry"].ToString());
                        usermodel.user_transfer = dataReader["user_transfer"].ToString();
                        usermodel.role_id = int.Parse(dataReader["roleid"].ToString());
                        usermodel.account = dataReader["account"].ToString();
                        usermodel.active = dataReader["active"].ToString();
                        usermodel.last_unssessful_login = dataReader["last_unsuccess_login"].ToString();
                        usermodel.company_name = dataReader["company_name"].ToString();
                        usermodel.user_custid = dataReader["user_custid"].ToString();
                        usermodel.login_status = dataReader["login_status"].ToString();
                    }
                }
                return usermodel;
            }
        }


        //---------------------Get Transfers Only---------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public List<AllTransfersViewModel> getMyTransfers(string user_id)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                //string query = "select tran_id,tran_name,tran_status,tran_resp_result from trans_log where user_id =" + user_id + "and ROWNUM <= 5 order by rownum desc";
                //string query =
                //    "select * from ( select tran_id , tran_name,tran_status,tran_resp_result from trans_log where user_id = '" +
                //    user_id + "' order by tran_id desc ) where rownum <= 5";

                string query = "select tran_id, tran_name, tran_status, tran_resp_result from trans_log where  user_id =" + user_id +
                    " and (tran_name = 'Own Transfer' or tran_name = 'To Bank Customer Transfer') order by tran_id desc";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();

                List<AllTransfersViewModel> list = new List<AllTransfersViewModel>();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        AllTransfersViewModel obj = new AllTransfersViewModel();

                        if (dataReader["tran_id"] != DBNull.Value)
                        {
                            //obj.AccountID = (int)dataReader["acc_id"];
                            obj.TranId = Convert.ToInt32(dataReader["tran_id"]);
                        }
                        if (dataReader["tran_name"] != DBNull.Value)
                        {
                            obj.TranName = (string)dataReader["tran_name"];
                        }
                        if (dataReader["tran_status"] != DBNull.Value)
                        {
                            obj.TranStatus = (string)dataReader["tran_status"];
                        }
                        if (dataReader["tran_resp_result"] != DBNull.Value)
                        {
                            obj.TranResult = (string)dataReader["tran_resp_result"];
                        }
                        list.Add(obj);
                    }

                    return list;

                }

            }

        }
        public List<CustomerAccounts> Custcounts(String bracode, String user_id)
        {
            OracleCommand cmd;
            OracleDataReader dr;

            String userid, username, useract, newuseract, newuseractcomplete;
            String query1, result;

            List<CustomerAccounts> customer = new List<CustomerAccounts>();

            if (!bracode.Equals("000"))
            {
                query1 = "select user_acc_link.user_id,user_name,b.branch_name||'-'||t.act_name||'-'||c.curr_name||'-'||SUBSTR(def_acc,14) def_acc,(select branch_name  from branchs where branch_code =SUBSTR(ACC_NO,3,3))||'-'||(select act_name  from act_types where act_type_code =SUBSTR(ACC_NO,6,5))||'-'||(select curr_name  from currency where  CURR_STS='1' and  CURR_CODE =SUBSTR(ACC_NO,11,3))||'-'||SUBSTR(ACC_NO,14) ,ACC_NO from users , user_acc_link ,branchs b ,act_types t , currency c    where     SUBSTR(def_acc,3,3)=b.branch_code and   SUBSTR(def_acc,6,5)=t.act_type_code and SUBSTR(def_acc,11,3)=c.CURR_CODE and   user_acc_link.user_id='" + user_id + "'and  and user_acc_link.user_id=users.user_id  and substr(def_acc,3,3)='" + bracode + "' order by user_id";
            }
            else
            {
                query1 = "select user_acc_link.user_id,user_name,b.branch_name||'-'||t.act_name||'-'||c.curr_name||'-'||SUBSTR(def_acc,14) def_acc,(select branch_name  from branchs where branch_code =SUBSTR(ACC_NO,3,3))||'-'||(select act_name  from act_types where act_type_code =SUBSTR(ACC_NO,6,5))||'-'||(select curr_name  from currency where  CURR_STS='1' and CURR_CODE =SUBSTR(ACC_NO,11,3))||'-'||SUBSTR(ACC_NO,14), ACC_NO from users , user_acc_link ,branchs b ,act_types t , currency c    where     SUBSTR(def_acc,3,3)=b.branch_code and   SUBSTR(def_acc,6,5)=t.act_type_code and SUBSTR(def_acc,11,3)=c.CURR_CODE and   user_acc_link.user_id='" + user_id + "'and   ACC_STATUS='A' and user_acc_link.user_id=users.user_id order by user_id";
            }

            using (OracleConnection con = new OracleConnection(conString))
            {
                cmd = new OracleCommand(query1, con);

                con.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        userid = dr[0].ToString();
                        username = dr[1].ToString();
                        useract = dr[2].ToString();
                        newuseract = dr[3].ToString();
                        newuseractcomplete = dr[4].ToString();

                        customer.Add(new CustomerAccounts
                        {
                            USER_ID = userid,
                            USER_NAME = username,
                            DEF_ACC = useract,
                            ACC_NO = newuseract,
                            ACC_NO1 = newuseractcomplete
                        });
                    }
                }


            }


            return customer;
        }


        //---------------------Get All Transactions---------------------------------
        public List<AllTransfersViewModel> getAllTransactions(string user_id)
        {
            using (OracleConnection con = new OracleConnection(conString))
            {
                //string query = "select tran_id,tran_name,tran_status,tran_resp_result from trans_log where user_id =" + user_id + "and ROWNUM <= 5 order by rownum desc";
                //string query =
                //    "select * from ( select tran_id , tran_name,tran_status,tran_resp_result from trans_log where user_id = '" +
                //    user_id + "' order by tran_id desc ) where rownum <= 5";

                string query = "select tran_id, tran_name, tran_status, tran_resp_result from trans_log where  user_id =" + user_id +
                               "order by tran_id desc";

                OracleCommand cmd = new OracleCommand(query, con);

                con.Open();

                List<AllTransfersViewModel> list = new List<AllTransfersViewModel>();
                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        AllTransfersViewModel obj = new AllTransfersViewModel();

                        if (dataReader["tran_id"] != DBNull.Value)
                        {
                            //obj.AccountID = (int)dataReader["acc_id"];
                            obj.TranId = Convert.ToInt32(dataReader["tran_id"]);
                        }
                        if (dataReader["tran_name"] != DBNull.Value)
                        {
                            obj.TranName = (string)dataReader["tran_name"];
                        }
                        if (dataReader["tran_status"] != DBNull.Value)
                        {
                            obj.TranStatus = (string)dataReader["tran_status"];
                        }
                        if (dataReader["tran_resp_result"] != DBNull.Value)
                        {
                            obj.TranResult = (string)dataReader["tran_resp_result"];
                        }
                        list.Add(obj);
                    }

                    return list;

                }

            }

        }
    }//class


    //protected string Encrypt(string clearText)
    //{
    //    //string EncryptionKey = "IBAZ2TWTQS77898";
    //    //byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
    //    //using (Aes encryptor = Aes.Create())
    //    //{
    //    //    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
    //    //    encryptor.Key = pdb.GetBytes(32);
    //    //    encryptor.IV = pdb.GetBytes(16);
    //    //    using (MemoryStream ms = new MemoryStream())
    //    //    {
    //    //        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
    //    //        {
    //    //            cs.Write(clearBytes, 0, clearBytes.Length);
    //    //            cs.Close();
    //    //        }
    //    //        clearText = Convert.ToBase64String(ms.ToArray());
    //    //    }
    //    //}
    //    CryptLib _crypt = new CryptLib();

    //    String key = "b16920894899c7780b5fc7161560a412";//CryptLib.SHA256("my secret key", 32); //32 bytes = 256 bit

    //    String iv = "e77886746a9b416d";
    //    //String iv = CryptLib.GenerateRandomIV(16); //16 bytes = 128 bits
    //    //string key = CryptLib.getHashSha256("my secret key", 31); //32 bytes = 256 bits
    //    String cypherText = _crypt.encrypt(clearText, key, iv);

    //    //Console.WriteLine("Plain text =" + _crypt.decrypt(cypherText, key, iv));
    //    return cypherText;
    //}

    //protected string Decrypt(string cipherText)
    //{
    //    string EncryptionKey = "IBAZ2TWTQS77898";
    //    byte[] cipherBytes = Convert.FromBase64String(cipherText);
    //    using (Aes encryptor = Aes.Create())
    //    {
    //        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
    //        encryptor.Key = pdb.GetBytes(32);
    //        encryptor.IV = pdb.GetBytes(16);
    //        using (MemoryStream ms = new MemoryStream())
    //        {
    //            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
    //            {
    //                cs.Write(cipherBytes, 0, cipherBytes.Length);
    //                cs.Close();
    //            }
    //            cipherText = Encoding.Unicode.GetString(ms.ToArray());
    //        }
    //    }
    //    return cipherText;
    //}

    public class Encryptor
    {

        public static string v;

        private static TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

        private static char[] ekey = "jaaaoiuyndfghjytewsdaaaa".ToCharArray();

        private static byte[] Key
        {
            get
            {
                return Encoding.Default.GetBytes(ekey);
                // Return Encoding.Default.GetBytes(WindowsIdentity.GetCurrent.Name.PadRight(24, Chr(0)))
            }
        }

        public static byte[] Vector
        {
            get
            {
                return Encoding.Default.GetBytes("fjhksjf9iufjsoifhihfsgdsgsg");
            }
        }

        public static string Encrypt(string Text)
        {
            return Encryptor.Transform(Text, des.CreateEncryptor(Key, Vector));
        }

        public static string Decrypt(string encryptedText)
        {
            return Encryptor.Transform(encryptedText, des.CreateDecryptor(Key, Vector));
        }

        private static string Transform(string Text, ICryptoTransform CryptoTransform)
        {
            MemoryStream stream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(stream, CryptoTransform, CryptoStreamMode.Write);
            byte[] Input = Encoding.Default.GetBytes(Text);
            cryptoStream.Write(Input, 0, Input.Length);
            cryptoStream.FlushFinalBlock();
            return Encoding.Default.GetString(stream.ToArray());
        }
    }
}
