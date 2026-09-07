using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using System.Data;
using Newtonsoft.Json;
using VAdvantage.Logging;


namespace VA005.Models
{
    public class Attribute
    {
        private Ctx _ctx = null;


        public Attribute(Ctx ctx)
        {
            _ctx = ctx;
        }

        public string SaveAttribute(VA005_SaveAttribute value)
        {
            MAttribute obj = new MAttribute(_ctx, value.ID, null);
            obj.SetName(value.name);


            if (value.description != null)
            {
                obj.SetDescription(value.description);
            }
            else
            {
                obj.SetDescription(" ");
            }


            if (value.attributetype != null)
            {
                obj.SetAttributeValueType(value.attributetype);
            }
            else
            {
                obj.SetAttributeValueType("L");
            }

            obj.SetIsMandatory(value.mandatory);
            obj.SetIsActive(value.isactivefield);
            obj.SetIsInstanceAttribute(value.istanceattribute);



            //obj.Save();
            if (obj.Save())
            {
                return obj.GetM_Attribute_ID().ToString();
            }
            // mattributeid = obj.GetM_Attribute_ID();
            //}
            //if (value.secname != null)            
            //{
            //    MAttributeValue objattval = new MAttributeValue(_ctx, 0, null);
            //    objattval.SetName(value.secname);
            //    objattval.SetValue(value.searchkey);
            //    objattval.SetM_Attribute_ID(mattributeid);
            //    if (objattval.Save())
            //    {
            //        return objattval.GetM_AttributeValue_ID().ToString();
            //    }
            //}            
            return Msg.GetMsg(_ctx, "VA005_UnableToSaveAttribute");
        }

        public string SelectionSave(VA005_SaveSelectionList value)
        {
            MAttributeValue obj = new MAttributeValue(_ctx, 0, null);
            obj.SetName(value.secname);

            if (String.IsNullOrEmpty(value.searchkey))
            {
                //obj.SetValue(value.secname);
                obj.SetValue(MSequence.GetDocumentNo(_ctx.GetAD_Client_ID(), "AD_Org", null, _ctx));
            }
            else
            {
                obj.SetValue(value.searchkey);
            }

            obj.SetM_Attribute_ID(value.attributeID);
            // Added by Shifali on 16th July 2020 to set seq no
            string str = "SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM M_AttributeValue WHERE M_Attribute_ID=" + obj.GetM_Attribute_ID();
            obj.Set_Value("SeqNo", Util.GetValueOfInt(DB.ExecuteScalar(str, null, null)));
            if (obj.Save())
            {
                return obj.GetM_AttributeValue_ID().ToString();
            }
            return Msg.GetMsg(_ctx, "VA005_UnableToSelectionSave");
        }

        public List<VA005_ShowGrideData> ShowGrideData(VA005_ShowGrideData value)
        {
            List<VA005_ShowGrideData> obj = new List<VA005_ShowGrideData>();
            //string sql = "select name,value,m_attributevalue_id from m_attributevalue where m_attribute_id='" + value.attributevalueid + "' and isactive='Y'";
            string sql = "select name,value,m_attributevalue_id,seqno from m_attributevalue where m_attribute_id='" + value.attributevalueid + "' and isactive='Y' ORDER BY M_Attributevalue_ID DESC";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj.Add(new VA005_ShowGrideData()
                    {
                        searchkey = Convert.ToString(ds.Tables[0].Rows[i]["value"]),
                        name = Convert.ToString(ds.Tables[0].Rows[i]["name"]),
                        attributevalueid = Convert.ToInt32(ds.Tables[0].Rows[i]["m_attributevalue_id"])
                    });
                }
            }
            return obj;
        }

        //public string SaveAttributeuses(VA005_SaveAttributeuses value)
        //{
        //    MAttributeUse obj = new MAttributeUse(_ctx, 0, null);
        //    //obj.SetM_AttributeSet_ID(value.attributsetid);
        //    //obj.SetM_Attribute_ID(value.lablename);

        //    obj.SetM_AttributeSet_ID(value.lablename);
        //    obj.SetM_Attribute_ID(value.attributsetid);

        //    var sql = "select max(seqno) as seqno from M_AttributeUse where isactive='Y'";
        //    DataSet ds = DB.ExecuteDataset(sql, null, null);
        //    value.sqno = Util.GetValueOfInt(ds.Tables[0].Rows[0]["seqno"]);

        //    obj.SetSeqNo(value.sqno + 10);
        //    if (obj.Save())
        //    {
        //        //return obj.GetM_AttributeSet_ID().ToString();
        //        return obj.GetM_Attribute_ID().ToString();
        //    }
        //    return Msg.GetMsg(_ctx, "VA005_UnableToSaveAttributeUses");
        //}
        // Added by Shifali on 04 Aug 2020 to Save multiple attributes
        /// <summary>
        ///  Save Attribute Uses
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="attributsetid">attributesetid</param>
        /// <param name="nid">node id</param>
        /// <returns>Result</returns>
        public string SaveAttributeuses(Ctx ctx, string attributsetid, int nid)
        {
            string[] attributeID = attributsetid.Split(',');
            for (int i = 0; i < attributeID.Length; i++)
            {
                MAttributeUse obj = new MAttributeUse(_ctx, 0, null);
                obj.SetM_AttributeSet_ID(nid);
                obj.SetM_Attribute_ID(Util.GetValueOfInt(attributeID[i]));
                var sql = "SELECT max(seqno) AS seqno FROM M_AttributeUse where isactive='Y' AND M_AttributeSet_ID=" + obj.GetM_AttributeSet_ID();
                DataSet ds = DB.ExecuteDataset(sql, null, null);
                var sqno = Util.GetValueOfInt(ds.Tables[0].Rows[0]["seqno"]);

                obj.SetSeqNo(sqno + 10);
                if (!obj.Save())
                {
                    //return obj.GetM_AttributeSet_ID().ToString();
                    return Msg.GetMsg(_ctx, "VA005_UnableToSaveAttributeUses");
                }
            }
            return string.Empty;
        }

        public String DeleteAttributeValue(VA005_DeleteAttributeValue value)
        {
            MAttributeValue obj = new MAttributeValue(_ctx, value.attributeID, null);
            int attvalid = obj.GetM_AttributeValue_ID();
            string _result = "";
            //try
            //{
            if (!obj.Delete(true))
            {
                ValueNamePair pp = VLogger.RetrieveError();
                _result = pp.ToString();
            }
            //}
            //catch(Exception e)
            //{
            //    return e.Message;
            //}
            //if (_result)
            //{
            //    //return Msg.GetMsg(_ctx,"VA005_Deleted");
            //    return attvalid.ToString();
            //}
            //else
            //{
            //return Msg.GetMsg(_ctx,"VA005_NotDeleted");
            //}
            return _result;
        }

        /// <summary>
        /// LoadSelectAttribute
        /// </summary>
        /// <returns>Load Select </returns>
        public List<ValueNamePair> LoadSelectAttribute()
        {
            List<ValueNamePair> Type = new List<ValueNamePair>();
            string sql = "SELECT a.Name,a.Value FROM AD_Ref_List a INNER JOIN AD_Reference b ON a.AD_Reference_ID=b.Ad_Reference_ID WHERE b.Name='M_Attribute Value Type' AND a.IsActive='Y'";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ValueNamePair dep = new ValueNamePair(Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]), Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]));
                //dep.Key = Util.GetValueOfString(ds.Tables[0].Rows[i]["value"]);
                //dep.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["name"]);

                Type.Add(dep);
            }
            return Type;
        }
        /// <summary>
        /// Load Attribute
        /// </summary>
        /// <param name="ID">ID</param>
        /// <returns>Load Attribute</returns>
        public List<AttributeDatas> LoadAttribute(int ID)
        {
            List<AttributeDatas> Type = new List<AttributeDatas>();
            string sql = "SELECT a.Name,  a.DESCRIPTION,a.IsActive, a.IsMandatory,  a.ATTRIBUTEVALUETYPE,  m.Value  FROM M_Attribute a LEFT OUTER  JOIN M_Attributevalue m  ON a.M_Attribute_ID  =m.M_Attribute_ID WHERE a.M_Attribute_ID =" + ID;
            var ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AttributeDatas dep = new AttributeDatas();
                    dep.name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    dep.description = Util.GetValueOfString(ds.Tables[0].Rows[i]["DESCRIPTION"]);
                    dep.isactive = Util.GetValueOfString(ds.Tables[0].Rows[i]["IsActive"]);
                    dep.ismandatory = Util.GetValueOfString(ds.Tables[0].Rows[i]["IsMandatory"]);
                    dep.attributevaluetype = Util.GetValueOfString(ds.Tables[0].Rows[i]["ATTRIBUTEVALUETYPE"]);
                    dep.value = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                    Type.Add(dep);
                }
            }
            return Type;
        }
        /// <summary>
        /// GetFieldLength
        /// </summary>
        /// <param name="TableID">TableID</param>
        /// <param name="COLUMNNAME">COLUMNNAME</param>
        /// <returns>Field length and column </returns>
        public List<ColumnData> GetFieldLength(int TableID, string COLUMNNAME)
        {
            List<ColumnData> Type = new List<ColumnData>();
            string sql = "SELECT Fieldlength,ColumnName FROM AD_Column WHERE AD_Table_ID =" + TableID + "  AND COLUMNNAME  IN (" + COLUMNNAME + ") AND isActive = 'Y'";
            var ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ColumnData dep = new ColumnData();
                    dep.fieldlength = Util.GetValueOfString(ds.Tables[0].Rows[i]["Fieldlength"]);
                    dep.columnname = Util.GetValueOfString(ds.Tables[0].Rows[i]["ColumnName"]);
                    Type.Add(dep);
                }
            }
            return Type;
        }
        /// <summary>
        /// Field Length
        /// </summary>
        /// <param name="TableID">TableID</param>
        /// <param name="COLUMNNAME">COLUMNNAME</param>
        /// <returns>Field length and column</returns>
        public List<ColumnData> GetField(int TableID, string COLUMNNAME)
        {
            List<ColumnData> Type = new List<ColumnData>();
            string sql = "SELECT Fieldlength,ColumnName FROM AD_Column WHERE AD_Table_ID =" + TableID + "  AND COLUMNNAME  IN (" + COLUMNNAME + ") AND isActive = 'Y'";
            var ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ColumnData dep = new ColumnData();
                    dep.fieldlength = Util.GetValueOfString(ds.Tables[0].Rows[i]["Fieldlength"]);
                    dep.columnname = Util.GetValueOfString(ds.Tables[0].Rows[i]["ColumnName"]);
                    Type.Add(dep);
                }
            }
            return Type;
        }
        /// <summary>
        /// Update Attribute
        /// </summary>
        /// <param name="fields">fields</param>
        /// <returns>Update Attribute</returns>
        public string Update(string fields)
        {
            string[] paramValue = fields.Split(',');
            string sql = "UPDATE M_Attributevalue SET Name='" + paramValue[0] + "',Value='" + paramValue[1] + "' WHERE M_Attributevalue_ID=" + paramValue[2];
            string attribute = Util.GetValueOfString(DB.ExecuteQuery(sql));
            return attribute;
        }
    }
        /// <summary>
        /// Declares Properties For ColumnData
        /// </summary>
        public class ColumnData
        {
            public string fieldlength { get; set; }
            public string columnname { get; set; }

        }
        /// <summary>
        /// Declares Properties For Attribute Data
        /// </summary>
        public class AttributeDatas
        {
            public string name { get; set; }
            public string description { get; set; }
            public string isactive { get; set; }
            public string ismandatory { get; set; }
            public string attributevaluetype { get; set; }
            public string value { get; set; }
        }
        public class VA005_SaveAttribute
        {
            public string name { get; set; }
            public string description { get; set; }
            public string attributetype { get; set; }
            public int ID { get; set; }

            public bool mandatory { get; set; }
            public bool isactivefield { get; set; }
            public bool istanceattribute { get; set; }

            //public string searchkey {get;set;}
            //public string secname { get; set; }
        }

        public class VA005_SaveSelectionList
        {
            public string searchkey { get; set; }
            public string secname { get; set; }
            public int attributeID { get; set; }
            // public int attributename { get; set; }
        }

        public class VA005_ShowGrideData
        {
            public string searchkey { get; set; }
            public string name { get; set; }
            public int attributevalueid { get; set; }
        }

        public class VA005_SaveAttributeuses
        {
            public int attributsetid { get; set; }
            public int lablename { get; set; }
            public int sqno { get; set; }
        }

        public class VA005_DeleteAttributeValue
        {
            public int attributeID { get; set; }

        }
    }

