// <auto-generated />
//
// This file was automatically generated by PocosGenerator.csx, inspired from the PetaPoco T4 Template
// Do not make changes directly to this file - edit the PocosGenerator.GenerateClass() method in the PocosGenerator.Core.csx file instead
// 

using System;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;

namespace com.apthai.CRMMobile.Model.CRMMobile
{
  

   [Table("DOC.DocumentDetailLevel2")]
    public partial class DocumentDetailLevel2
    {
        [Key]
        public int DocumentDetailID { get; set; }
        public int RefDocumentHeaderID { get; set; }
        public string DocumentDetailName { get; set; }
        public int OrderOfDocumentDetail { get; set; }
        public string URLDocumentDetail { get; set; }
        public string StatusDocumentDetail { get; set; }
        public string Created { get; set; }
        public string CreatedBy { get; set; }
        public string Updated { get; set; }
        public string UpdatedBy { get; set; }
    }

   [Table("DOC.DocumentHeaderLevel1")]
    public partial class DocumentHeaderLevel1
    {
        [Key]
        public int DocumentHeaderID { get; set; }
        public string DocumentHeaderName { get; set; }
        public int OrderOfDocumentHeader { get; set; }
        public string StatusDocumentHeader { get; set; }
        public string URLDocumentHeader { get; set; }
        public string Created { get; set; }
        public string CreatedBy { get; set; }
        public string Updated { get; set; }
        public string UpdatedBy { get; set; }
    }

   [Table("MST.MappingURLProject")]
    public partial class MappingURLProject
    {
        [Key]
        public int URLID { get; set; }
        public string URLPicture { get; set; }
        public Guid? ProjectID { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }

   [Table("NT.NotificationHistory")]
    public partial class NotificationHistory
    {
        [Key]
        public int NotiHistoryID { get; set; }
        public string CRMContactID { get; set; }
        public string ReceiptNo { get; set; }
        public string BillingCreditNo { get; set; }
        public string BillingDebitNo { get; set; }
        public string RefID { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectNameTH { get; set; }
        public string UnitNo { get; set; }
        public string MsgTH { get; set; }
        public string MsgEN { get; set; }
        public string MsgType { get; set; }
        public bool SendMsgStatus { get; set; }
        public string Created { get; set; }
        public string CreatedBy { get; set; }
        public string Updated { get; set; }
        public string UpdatedBy { get; set; }
    }

   [Table("NT.NotificationTemp")]
    public partial class NotificationTemp
    {
        [Key]
        public int NotiTempID { get; set; }
        public string CRMContactID { get; set; }
        public string ReceiptNo { get; set; }
        public string BillingCreditNo { get; set; }
        public string BillingDebitNo { get; set; }
        public string RefID { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectNameTH { get; set; }
        public string UnitNo { get; set; }
        public string MsgTH { get; set; }
        public string MsgEN { get; set; }
        public string MsgType { get; set; }
        public bool MsgStatus { get; set; }
        public string Created { get; set; }
        public string CreatedBy { get; set; }
        public string Updated { get; set; }
        public string UpdatedBy { get; set; }
    }

   [Table("NT.NotificationType")]
    public partial class NotificationType
    {
        [Key]
        public int NotiTypeID { get; set; }
        public string NotiType { get; set; }
        public string Created { get; set; }
        public string CreatedBy { get; set; }
        public string Updated { get; set; }
        public string UpdatedBy { get; set; }
    }

   [Table("MST.ProjectLocation")]
    public partial class ProjectLocation
    {
        [Key]
        public int LocationID { get; set; }
        public int? ProjectID { get; set; }
        public string ProjectNo { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }

   [Table("DOC.SubDocumentDetailLevel3")]
    public partial class SubDocumentDetailLevel3
    {
        [Key]
        public int SubDocumentDetailID { get; set; }
        public int RefDocumentDetailID { get; set; }
        public string SubDocumentDetailName { get; set; }
        public int OrderOfSubDocumentDetail { get; set; }
        public string StatusSubDocumentDetail { get; set; }
        public string URLSubDocumentDetail { get; set; }
        public string Created { get; set; }
        public string CreatedBy { get; set; }
        public string Updated { get; set; }
        public string UpdatedBy { get; set; }
    }

   [Table("Table_1")]
    public partial class Table1
    {
        public string ID { get; set; }
        public string name { get; set; }
    }

   [Table("CS.UserLogin")]
    public partial class UserLogin
    {
        [Key]
        public int UserLoginID { get; set; }
        public string UserPhoneNumber { get; set; }
        public string LoginDate { get; set; }
        public string DeviceID { get; set; }
        public string DeviceType { get; set; }
        public string UserToken { get; set; }
        public string FireBaseToken { get; set; }
        public int? UserProfileID { get; set; }
    }

   [Table("CS.UserProfile")]
    public partial class UserProfile
    {
        [Key]
        public int UserProfileID { get; set; }
        public string CRMContactID { get; set; }
        public string TitleExtTH { get; set; }
        public string FirstNameTH { get; set; }
        public string MiddleNameTH { get; set; }
        public string LastNameTH { get; set; }
        public string Nickname { get; set; }
        public string TitleExtEN { get; set; }
        public string FirstNameEN { get; set; }
        public string MiddleNameEN { get; set; }
        public string LastNameEN { get; set; }
        public string CitizenIdentityNo { get; set; }
        public string PINCode { get; set; }
        public bool IsActive { get; set; }
        public string Created { get; set; }
        public string CreatedBy { get; set; }
        public string Updated { get; set; }
        public string UpdatedBy { get; set; }
    }
}
