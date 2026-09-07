/********************************************************
 * Module Name    : VA003
 * Purpose        : Create Attribute Set Listing
 * Class Used     : 
 * Karan          : 29 July 2015
 * Manish         : 01 July 2015  
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DataContracts;

namespace VA005.Models
{
    public class AttributeListing
    {
        private Ctx _ctx = null;

        public AttributeListing(Ctx ctx)
        {
            _ctx = ctx;
        }

        string exp = "";
        /// <summary>
        /// USed to Create Tree. This is main function which intialize tree creation.
        /// </summary>
        public object CreateTree(string Expend)
        {
            exp = Expend;
            StringBuilder sql = new StringBuilder();

            sql.Append(MRole.GetDefault(_ctx).AddAccessSQL("SELECT m_attributeSet_ID , Name FROM m_attributeSet WHERE IsActive='Y' ORDER BY LOWER(Name),m_attributeSet_ID", "M_AttributeSet", true, true));
            sql.Append("~");
            sql.Append(MRole.GetDefault(_ctx).AddAccessSQL(@"SELECT m_attribute.m_attribute_ID,m_attribute.Name,m_attributeuse.m_attributeset_id,m_attribute.ATTRIBUTEVALUETYPE
                                        FROM m_attributeuse
                                        JOIN m_attribute
                                        ON m_attributeuse.m_attribute_ID=m_attribute.m_attribute_ID WHERE m_attributeuse.IsActive='Y' AND m_attributeuse.IsActive='Y' ORDER BY m_attribute.Name,m_attribute.m_attribute_ID", "M_AttributeUse", true, true));
            sql.Append("~");
            sql.Append(MRole.GetDefault(_ctx).AddAccessSQL("SELECT m_attribute_ID , name,  M_AttributeValue_ID FROM M_AttributeValue WHERE IsActive='Y' ORDER BY name,m_attribute_ID  ", "M_AttributeValue", true, true));
            sql.Append("~");

            SqlParamsIn sqlpar = new SqlParamsIn();
            sqlpar.sql = sql.ToString();
            sqlpar.pageSize = 0;

            VIS.Helpers.SqlHelper sHelper = new VIS.Helpers.SqlHelper();

            DataSet ds = sHelper.ExecuteDataSet(sqlpar);

            if (ds == null || ds.Tables.Count < 1)
            {
                return null;
            }

            VA005_TreeStructure tree = new VA005_TreeStructure();
            tree.text = Msg.GetMsg(_ctx, "VA005_AttributeSetListing");
            tree.NodeID = 0;
            //tree.ParentID = 0;
            tree.expanded = true;
            tree.visibility = "none";
            tree.ShowInfo = "none";
            tree.padding = "8px 10px 8px 10px";
            tree.margin = "0 0 0 0";

            //tree.ImageSource = "Areas/VA005/Images/1.png";            
            tree.items = new List<VA005_TreeStructure>();
            LoadAttributeSet(ds, tree);
            //hidden
            List<VA005_TreeStructure> final = new List<VA005_TreeStructure>();
            final.Add(tree);


            return final;
        }

        /// <summary>
        /// Create Attribute set and then fill each attribute set with respective Attribute used in it.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="parentTree"></param>
        public void LoadAttributeSet(DataSet ds, VA005_TreeStructure parentTree)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {



                VA005_TreeStructure attributeSetTree = new VA005_TreeStructure();
                attributeSetTree.text = System.Net.WebUtility.HtmlEncode(Convert.ToString(ds.Tables[0].Rows[i]["Name"]));
                attributeSetTree.NodeID = Convert.ToInt32(ds.Tables[0].Rows[i]["m_attributeSet_ID"]);
                //attributeSetTree.ParentID = Convert.ToInt32(ds.Tables[0].Rows[i]["m_attributeSet_ID"]);
                //attributeSetTree.ImageSource = "Areas/VA005/Images/attSet.png";
                attributeSetTree.ImageSource = "vis vis-product";
                //attributeSetTree.Image2 = "Areas/VA005/Images/edit.png";
                attributeSetTree.Image2 = "Areas/VA005/Images/edt.png";
                attributeSetTree.visibility = "inherit";
                attributeSetTree.ShowInfo = "none";
                attributeSetTree.padding = "8px 10px 8px 38px";
                attributeSetTree.margin = "0 0 0 0";
                attributeSetTree.Type = "1";
                attributeSetTree.UID = attributeSetTree.NodeID + "_1_" + exp;
                attributeSetTree.NID = attributeSetTree.NodeID;
                //attributeSetTree.UID = attributeSetTree.NodeID.ToString();
                attributeSetTree.expanded = false;
                attributeSetTree.zindex = "99999";
                attributeSetTree.classforgetnod = "classforgetnod";
                attributeSetTree.items = new List<VA005_TreeStructure>();
                parentTree.items.Add(attributeSetTree);
                LoadAttributeUse(ds, attributeSetTree, Convert.ToInt32(ds.Tables[0].Rows[i]["m_attributeSet_ID"]));
            }
        }
        /// <summary>
        ///  Create Attribute Used in each attribute Set and then fill each attribute Use with respective Value.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="parentTree"></param>
        /// <param name="parentID"></param>
        public void LoadAttributeUse(DataSet ds, VA005_TreeStructure parentTree, int parentID)
        {
            DataRow[] drs = ds.Tables[1].Select("m_attributeset_id=" + parentID);

            if (drs != null && drs.Length > 0)
            {
                for (int j = 0; j < drs.Length; j++)
                {
                    VA005_TreeStructure attributeSetTree = new VA005_TreeStructure();
                    attributeSetTree.text = System.Net.WebUtility.HtmlEncode(Convert.ToString(drs[j]["Name"]));
                    ////attributeSetTree.NodeID = Convert.ToInt32(drs[j]["m_attributeSet_ID"]);===================
                    attributeSetTree.NodeID = Convert.ToInt32(drs[j]["m_attribute_ID"]);
                    attributeSetTree.ParentID = parentID;
                    //attributeSetTree.ImageSource = "Areas/VA005/Images/1.png";
                    //attributeSetTree.ImageSource = "Areas/VA005/Images/att.png";
                    attributeSetTree.ImageSource = "vis vis-task";

                    attributeSetTree.Image2 = "Areas/VA005/Images/edit.png";
                    //                    attributeSetTree.Image3 = "Areas/VA005/Images/multi-sel2.png";
                    attributeSetTree.Image3 = "Areas/VA005/Images/infop.png";
                    attributeSetTree.visibility = "inherit";
                    attributeSetTree.ShowInfo = "inherit";
                    attributeSetTree.padding = "8px 10px 8px 38px";
                    attributeSetTree.margin = "0 0 0 15px";
                    attributeSetTree.Type = "2";
                    attributeSetTree.expanded = false;
                    attributeSetTree.zindex = "99999";
                    attributeSetTree.UID = attributeSetTree.NodeID + "_2_" + exp;
                    attributeSetTree.NID = attributeSetTree.NodeID;
                    //attributeSetTree.UID = attributeSetTree.NodeID.ToString();
                    attributeSetTree.items = new List<VA005_TreeStructure>();
                    parentTree.items.Add(attributeSetTree);

                    if (Convert.ToString(drs[j]["ATTRIBUTEVALUETYPE"]) == "L")
                    {
                        LoadAttributeValue(ds, attributeSetTree, Convert.ToInt32(drs[j]["m_attribute_ID"]));
                    }
                }
            }
        }

        /// <summary>
        /// Create Attribute Values
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="parentTree"></param>
        /// <param name="parentID"></param>
        public void LoadAttributeValue(DataSet ds, VA005_TreeStructure parentTree, int parentID)
        {
            DataRow[] drs = ds.Tables[2].Select("m_attribute_id=" + parentID);
            if (drs != null && drs.Length > 0)
            {
                for (int j = 0; j < drs.Length; j++)
                {
                    VA005_TreeStructure attributeSetTree = new VA005_TreeStructure();
                    attributeSetTree.text = System.Net.WebUtility.HtmlEncode(Convert.ToString(drs[j]["Name"]));
                    attributeSetTree.NodeID = Convert.ToInt32(drs[j]["M_AttributeValue_ID"]);
                    //  attributeSetTree.ParentID = 0;
                    attributeSetTree.visibility = "none";
                    attributeSetTree.ShowInfo = "none";
                    attributeSetTree.padding = "8px 10px 8px 38px";
                    attributeSetTree.margin = "0 0 0 20px";
                    attributeSetTree.Type = "3";
                    attributeSetTree.zindex = "99999";
                    attributeSetTree.UID = attributeSetTree.NodeID + "_3_" + exp;
                    attributeSetTree.NID = attributeSetTree.NodeID;
                    //attributeSetTree.UID = attributeSetTree.NodeID.ToString();
                    attributeSetTree.expanded = false;
                    parentTree.items.Add(attributeSetTree);
                }
            }
        }

        public string CreateNewLot(VA005_LotCtrl values)
        {
            MLotCtl ctrl = new MLotCtl(_ctx, 0, null);
            ctrl.SetName(values.Name);
            ctrl.SetStartNo(Convert.ToInt32(values.StartNo));
            ctrl.SetIncrementNo(Convert.ToInt32(values.Increment));
            ctrl.SetCurrentNext(Convert.ToInt32(values.CurrentNext));
            ctrl.SetPrefix(values.prefix);
            ctrl.SetSuffix(values.Suffix);
            if (ctrl.Save())
            {
                return ctrl.GetM_LotCtl_ID().ToString();
            }
            return Msg.GetMsg(_ctx, "VA005_UnableToSaveLot");
        }
        //=============================================================================================================

        /// <summary>
        /// Add Attribute Set Value
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public string AddAttributeSet(VA005_AddAttributeSet values)
        {
            MAttributeSet ctrl = new MAttributeSet(_ctx, values.ID, null);
            ctrl.SetName(values.name);
            ctrl.SetDescription(values.description);
            ctrl.SetMandatoryType(values.mandatorytype);
            ctrl.SetIsGuaranteeDate(values.IsGuaranteeDate);
            ctrl.SetIsGuaranteeDateMandatory(values.IsGuaranteeDateMandatory);
            ctrl.SetIsLot(values.islotcheck);
            ctrl.SetIsSerNo(values.isserialcheck);
            if (!String.IsNullOrEmpty(values.lotvalue))
            {
                ctrl.SetM_LotCtl_ID(Convert.ToInt32(values.lotvalue));
            }
            else
            {
                ctrl.SetM_LotCtl_ID(0);
            }

            if (!String.IsNullOrEmpty(values.serialvalue))
            {
                ctrl.SetM_SerNoCtl_ID(Convert.ToInt32(values.serialvalue));
            }
            else
            {
                ctrl.SetM_SerNoCtl_ID(0);
            }

            if (ctrl.Save())
            {
                return ctrl.GetM_AttributeSet_ID().ToString();
            }
            return Msg.GetMsg(_ctx, "VA005_UnableToAddAttributeSet");
        }

        /// <summary>
        /// Add New Serial....
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public string CresteNewSerial(VA005_SerialCtr values)
        {
            MSerNoCtl ctrl = new MSerNoCtl(_ctx, 0, null);
            ctrl.SetName(values.name);
            ctrl.SetStartNo(Convert.ToInt32(values.startno));
            ctrl.SetCurrentNext(Convert.ToInt32(values.currentnext));
            ctrl.SetIncrementNo(Convert.ToInt32(values.incrementno));
            ctrl.SetPrefix(values.prefix);
            ctrl.SetSuffix(values.sufix);
            if (ctrl.Save())
            {
                return ctrl.GetM_SerNoCtl_ID().ToString();
            }
            return Msg.GetMsg(_ctx, "VA005_UnableToSaveSerial");
        }

        /// <summary>
        /// Add New Selection List...
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public string AddNewAttribute(VA005_AddNewAttribute values)
        {
            MAttribute ctrl = new MAttribute(_ctx, 0, null);
            ctrl.SetName(values.Name);
            ctrl.SetDescription(values.Description);
            ctrl.SetM_AttributeSearch_ID(values.M_AttributeSearch_ID);
            if (ctrl.Save())
            {
                return ctrl.GetM_Attribute_ID().ToString();
            }
            return Msg.GetMsg(_ctx, "VA005_UnableToAddNewAttribute");
        }

        /// <summary>
        /// Add Attribute Type for selection.....
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public string AddNewAttributeType(VA005_AddNewAttributeType values)
        {
            MAttributeValue ctrl = new MAttributeValue(_ctx, 0, null);
            ctrl.SetValue(values.Value);
            ctrl.SetName(values.Name);
            ctrl.SetDescription(values.Description);
            ctrl.SetM_Attribute_ID(values.M_Attribute_ID);
            if (ctrl.Save())
            {
                return ctrl.GetM_AttributeValue_ID().ToString();
            }
            return Msg.GetMsg(_ctx, "VA005_UnableToAddNewAttributeValue");
        }

        /// <summary>
        /// Attribute Listing Tree..
        /// </summary>
        /// <returns></returns>
        public object createTreeAttributeName()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(MRole.GetDefault(_ctx).AddAccessSQL("SELECT M_Attribute_id  , Name FROM M_Attribute  WHERE IsActive='Y'", "M_Attribute", true, false));
            sql.Append("~");
            sql.Append(MRole.GetDefault(_ctx).AddAccessSQL("SELECT ma.M_Attribute_id,mv.m_attributevalue_id, ma.Name as NameAttribute,mv.Name FROM M_Attribute ma inner JOIN m_attributevalue mv ON ma.M_Attribute_id=mv.M_Attribute_id", "M_Attribute", true, false));

            SqlParamsIn sqlpar = new SqlParamsIn();
            sqlpar.sql = sql.ToString();
            sqlpar.pageSize = 0;

            VIS.Helpers.SqlHelper sHelper = new VIS.Helpers.SqlHelper();

            DataSet ds = sHelper.ExecuteDataSet(sqlpar);

            if (ds == null || ds.Tables.Count < 1)
            {
                return null;
            }

            VA005_TreeStructure tree = new VA005_TreeStructure();
            tree.text = Msg.GetMsg(_ctx, "VA005_AttributeTree");
            tree.NodeID = 0;
            tree.items = new List<VA005_TreeStructure>();
            LoadAttributeName(ds, tree);

            List<VA005_TreeStructure> final = new List<VA005_TreeStructure>();
            final.Add(tree);
            return final;
        }
        public void LoadAttributeName(DataSet ds, VA005_TreeStructure parentTree)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                VA005_TreeStructure attributeSetTree = new VA005_TreeStructure();
                attributeSetTree.text = Convert.ToString(ds.Tables[0].Rows[i]["Name"]);
                attributeSetTree.NodeID = Convert.ToInt32(ds.Tables[0].Rows[i]["M_Attribute_id"]);
                attributeSetTree.items = new List<VA005_TreeStructure>();
                parentTree.items.Add(attributeSetTree);
                LoadAttributeItems(ds, attributeSetTree, Convert.ToInt32(ds.Tables[0].Rows[i]["M_Attribute_id"]));
            }
        }
        public void LoadAttributeItems(DataSet ds, VA005_TreeStructure parentTree, int parentID)
        {
            DataRow[] drs = ds.Tables[1].Select("M_Attribute_id=" + parentID);

            if (drs != null && drs.Length > 0)
            {
                for (int j = 0; j < drs.Length; j++)
                {
                    VA005_TreeStructure attributeSetTree = new VA005_TreeStructure();
                    attributeSetTree.text = Convert.ToString(drs[j]["Name"]);
                    attributeSetTree.NodeID = Convert.ToInt32(drs[j]["M_Attribute_id"]);
                    attributeSetTree.items = new List<VA005_TreeStructure>();
                    parentTree.items.Add(attributeSetTree);
                }
            }
        }
        /// <summary>
        /// Get Attribute Value on Btn Click...........
        /// </summary>
        /// <returns></returns>
        public List<VA005_AddAttributeSet> Attributegetdataonbtclick()
        {
            List<VA005_AddAttributeSet> obj = new List<VA005_AddAttributeSet>();
            string sql = "select m_attributeset_id, Name,description,mandatorytype,IsGuaranteeDate,IsGuaranteeDateMandatory from m_attributeset where m_attributeset_id=m_attributeset_id";
            DataSet ds = DB.ExecuteDataset(sql, null, null);

            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        obj.Add(new VA005_AddAttributeSet()
                        {
                            name = Convert.ToString(ds.Tables[0].Rows[i]["Name"]),
                            description = Convert.ToString(ds.Tables[0].Rows[i]["description"]),
                            mandatorytype = Convert.ToString(ds.Tables[0].Rows[i]["mandatorytype"]),
                            IsGuaranteeDate = Convert.ToBoolean(ds.Tables[0].Rows[i]["IsGuaranteeDate"]),
                            IsGuaranteeDateMandatory = Convert.ToBoolean(ds.Tables[0].Rows[i]["IsGuaranteeDateMandatory"]),
                        });
                    }
                }
            }
            return obj;
        }
        /// <summary>
        /// Get Value for VA005_GetLoadAttributeName......
        /// </summary>
        /// <returns></returns>
        public List<VA005_GetLoadAttributeName> Attributegetdataonnameclick()
        {
            List<VA005_GetLoadAttributeName> obj = new List<VA005_GetLoadAttributeName>();
            string sql = "select m_attribute_id, Name from m_attribute where m_attribute_id=m_attribute_id;";
            DataSet ds = DB.ExecuteDataset(sql, null, null);

            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        obj.Add(new VA005_GetLoadAttributeName()
                        {
                            name = Convert.ToString(ds.Tables[0].Rows[i]["Name"]),
                            id = Convert.ToInt32(ds.Tables[0].Rows[i]["m_attribute_id"]),
                        });
                    }
                }
            }
            return obj;
        }
        /// <summary>
        /// Get Attribute Name By Icon......
        /// </summary>
        /// <returns></returns>
        public List<VA005_GetAttributeNameByIcon> Attributegetdataoniconclick()
        {
            List<VA005_GetAttributeNameByIcon> obj = new List<VA005_GetAttributeNameByIcon>();
            string sql = "select m_attributeset_id, Name from m_attributeset";
            DataSet ds = DB.ExecuteDataset(sql, null, null);

            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        obj.Add(new VA005_GetAttributeNameByIcon()
                        {
                            name = Convert.ToString(ds.Tables[0].Rows[i]["Name"]),
                            id = Convert.ToInt32(ds.Tables[0].Rows[i]["m_attributeset_id"]),
                        });
                    }
                }
            }
            return obj;
        }


        public string DeleteAttributeSetValue(VA005_DeleteAttributeSetValue value)
        {
            //MAttributeValue obj = new MAttributeValue(_ctx, value.attributesetdelID, null);
            MAttributeSet obj = new MAttributeSet(_ctx, value.attributesetdelID, null);
            int attvalid = obj.GetM_AttributeSet_ID();
            string _result = "";

            if (!obj.Delete(true))
            {
                ValueNamePair pp = VLogger.RetrieveError();
                _result = pp.ToString();
            }

            return _result;
        }


        //public string DeleteAttributeSetValue(VA005_DeleteAttributeSetValue value)
        //{
        //    //MAttributeValue obj = new MAttributeValue(_ctx, value.attributesetdelID, null);
        //    MAttributeSet obj = new MAttributeSet(_ctx, value.attributesetdelID, null);
        //    int attvalid = obj.GetM_AttributeSet_ID();
        //    bool _result = false;
        //    try
        //    {
        //        _result = obj.Delete(true);
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message;
        //    }
        //    if (_result)
        //    {
        //        //return Msg.GetMsg(_ctx,"VA005_Deleted");
        //        return attvalid.ToString();
        //    }
        //    else
        //    {
        //        return Msg.GetMsg(_ctx, "VA005_NotDeleted");
        //    }
        //}

        public List<KeyNamePair> DeleteAttributeSetValue(List<Int32> value)
        {
            //string _result = "";
            List<KeyNamePair> NameList = new List<KeyNamePair>();
            for (int i = 0; i < value.Count; i++)
            {
                MAttribute obj = new MAttribute(_ctx, value[i], null);
                int attvalid = obj.GetM_Attribute_ID();
                //try
                //{
                if (!obj.Delete(true))
                {
                    //ValueNamePair pp = VLogger.RetrieveError();
                    //_result += pp.ToString() + "/n";
                    // Added by Shifali on 27th July 2020 to get the names of attribute which are not deleted
                    KeyNamePair objkey = new KeyNamePair();
                    objkey.Key = Util.GetValueOfInt(value[i]);
                    objkey.Name = string.Empty;
                    string str = "SELECT Name FROM M_Attribute WHERE M_Attribute_ID = " + obj.GetM_Attribute_ID();
                    objkey.Name = Util.GetValueOfString(DB.ExecuteScalar(str, null, null));
                    NameList.Add(objkey);
                }
                //}
                //catch (Exception e)
                //{

                //}
            }
            return NameList;
        }
        /// <summary>
        /// Method For Load Attribute
        /// </summary>
        /// <returns>Load Grid</returns>
        public List<AttributeAppend> GetAttribute()
        {
            List<AttributeAppend> PPData = new List<AttributeAppend>();
            string sql = "SELECT Name, M_Attribute_ID,IsActive FROM M_Attribute  ORDER BY M_Attribute_ID DESC";
            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql, "M_Attribute", true, false);
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AttributeAppend PData = new AttributeAppend();
                    PData.m_attribute_id = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Attribute_ID"]);
                    PData.name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    PData.isactive = Util.GetValueOfString(ds.Tables[0].Rows[i]["isactive"]);
                    PPData.Add(PData);
                }
            }
            return PPData;
        }
        /// <summary>
        /// Method For Save Attribute
        /// </summary>
        /// <returns>Save The Attribute</returns>
        public int GetAttributeCount()
        {
            string sql = "SELECT COUNT(Name) as Name FROM M_Attribute WHERE IsActive='Y'";
            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql, "M_Attribute", true, false);
            int count = Util.GetValueOfInt(DB.ExecuteScalar(sql));
            return count;
        }
        /// <summary>
        /// Method For Get Attribute Value
        /// </summary>
        /// <param name="SelectAttributeID">SelectAttributeID</param>
        /// <returns>Load Attribute Grid</returns>
        public List<AttributeValue> GetAttributeValue(int SelectAttributeID)
        {
            List<AttributeValue> PPData = new List<AttributeValue>();
            string sql = "SELECT M_ATTRIBUTEVALUE_ID, Name, M_ATTRIBUTE_ID FROM M_Attributevalue WHERE M_ATTRIBUTE_ID=" + SelectAttributeID + " ORDER BY M_ATTRIBUTEVALUE_ID ";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AttributeValue PData = new AttributeValue();
                    PData.m_attributevalue_id = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_ATTRIBUTEVALUE_ID"]);
                    PData.name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    PData.m_attribute_id = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_ATTRIBUTE_ID"]);
                    PPData.Add(PData);
                }
            }
            return PPData;
        }
        /// <summary>
        /// Method For Edit Attribute list
        /// </summary>
        /// <param name="Control">Control</param>
        /// <returns>Load Window</returns>
        public int GetWindow_ID(string Control)
        {
            string sql = "SELECT AD_Window_ID FROM AD_Window WHERE Name='" + Control + "'";
            int ID = Util.GetValueOfInt(DB.ExecuteScalar(sql));
            return ID;
        }
        /// <summary>
        /// Method For Mandatory Drop Down
        /// </summary>
        /// <returns>Data Into Mandatory DropDown</returns>
        public List<ValueNamePair> GetMandatoryType()
        {
            List<ValueNamePair> Type = new List<ValueNamePair>();
            string sql = @"SELECT  AD_Ref_List.Name, AD_Ref_List.Value FROM AD_Ref_List JOIN AD_Reference ON AD_Ref_List.AD_Reference_ID=AD_Reference.AD_Reference_ID
                           WHERE AD_Reference.Name='M_AttributeSet MandatoryType' AND AD_Ref_List.IsActive='Y'";
            var ds = DB.ExecuteDataset(sql, null, null);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ValueNamePair dep = new ValueNamePair(Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]), Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]));
                //dep.Key = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                //dep.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);

                Type.Add(dep);
            }
            return Type;
        }
        /// <summary>
        /// Method For Lot Drop Down 
        /// </summary>
        /// <returns>Load Lot Drop Down</returns>
        public List<KeyNamePair> GetLotData()
        {
            List<KeyNamePair> Type = new List<KeyNamePair>();
            string sql = "SELECT M_LotCtl_ID,Name FROM M_LotCtl WHERE isActive='Y'";
            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql, "M_LotCtl", true, false);
            var ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    KeyNamePair dep = new KeyNamePair();
                    dep.Key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_LotCtl_ID"]);
                    dep.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    Type.Add(dep);
                }

            }
            return Type;
        }
        /// <summary>
        /// Method For Serial DropDown
        /// </summary>
        /// <returns>Get Data Into Serial DropDown</returns>
        public List<KeyNamePair> GetSerialData()
        {
            List<KeyNamePair> Type = new List<KeyNamePair>();
            string sql = "SELECT M_SerNoCtl_ID,Name FROM M_SerNoCtl WHERE IsActive='Y'";
            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql, "M_SerNoCtl", true, false);
            var ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    KeyNamePair dep = new KeyNamePair();
                    dep.Key = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_SerNoCtl_ID"]);
                    dep.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    Type.Add(dep);
                }

            }
            return Type;
        }
        /// <summary>
        /// Load Edit Attribute Set
        /// </summary>
        /// <param name="NodeID">Node ID</param>
        /// <returns>Load Edit Attribute Set</returns>
        public List<AttributeData> GetAttributeSetData(int NodeID)
        {
            List<AttributeData> Type = new List<AttributeData>();
            string sql = "SELECT NAME,DESCRIPTION,MANDATORYTYPE,ISGUARANTEEDATE,ISGUARANTEEDATEMANDATORY,M_LOTCTL_ID,M_SERNOCTL_ID,IsLot,IsSerNo FROM M_AttributeSet WHERE M_Attributeset_ID=" + NodeID;            
            var ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AttributeData dep = new AttributeData();
                    dep.name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    dep.description = Util.GetValueOfString(ds.Tables[0].Rows[i]["DESCRIPTION"]);
                    dep.mandatorytype = Util.GetValueOfString(ds.Tables[0].Rows[i]["MANDATORYTYPE"]);
                    dep.isguaranteedate = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISGUARANTEEDATE"]);
                    dep.isguaranteedatemandatory = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISGUARANTEEDATEMANDATORY"]);
                    dep.m_lotctl_id = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_LOTCTL_ID"]);
                    dep.m_sernoctl_id = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_SERNOCTL_ID"]);
                    dep.islot = Util.GetValueOfString(ds.Tables[0].Rows[i]["IsLot"]);
                    dep.isserno = Util.GetValueOfString(ds.Tables[0].Rows[i]["IsSerNo"]);
                    Type.Add(dep);
                }
            }
            return Type;
        }
        /// <summary>
        /// Remove Attribute Formatt
        /// </summary>
        /// <param name="AttributeID">Attribute ID</param>
        /// <param name="ParentId">Parent ID</param>
        /// <returns>Remove Attribute Formatt</returns>
        public int DeleteAttributeUse(int AttributeID, int ParentId)
        {
            string sql = "DELETE FROM M_Attributeuse WHERE M_Attribute_ID=" + AttributeID + " and M_Attributeset_ID=" + ParentId;
            int ID = Util.GetValueOfInt(DB.ExecuteQuery(sql));
            return ID;
        }
        /// <summary>
        /// Load Link Attribute
        /// </summary>
        /// <param name="AttributeID">Attribute ID</param>
        /// <returns>Load Link Attribute</returns>
        public List<string> LoadAttributeUse(int AttributeID)
        {
            List<string> Lst = new List<string>();
            string sql = "SELECT mas.Name FROM M_Attributeuse masu JOIN M_Attributeset mas on masu.M_Attributeset_ID=mas.M_Attributeset_ID WHERE masu.M_Attribute_ID=" + AttributeID;
            var ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Lst.Add(Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]));
                }
            }
            return Lst;
        }
        /// <summary>
        /// Field Length
        /// </summary>
        /// <param name="TableID">TableID</param>
        /// <param name="COLUMNNAME">COLUMNNAME</param>
        /// <returns>Column Name And Length</returns>
        public List<fieldlengthDetails> GetFieldLength(int TableID, string COLUMNNAME)
        {
            List<fieldlengthDetails> Type = new List<fieldlengthDetails>();
            string sql = "SELECT Fieldlength,ColumnName FROM AD_Column WHERE AD_Table_ID =" + TableID + "  AND COLUMNNAME  IN (" + COLUMNNAME + ") AND isActive = 'Y'";
            var ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    fieldlengthDetails dep = new fieldlengthDetails();
                    dep.fieldlength = Util.GetValueOfString(ds.Tables[0].Rows[i]["Fieldlength"]);
                    dep.columnname = Util.GetValueOfString(ds.Tables[0].Rows[i]["ColumnName"]);
                    Type.Add(dep);
                }
            }
            return Type;
        }
    }
        public class fieldlengthDetails
    {
            public string fieldlength { get; set; }
            public string columnname { get; set; }

        }

        public class VA005_AddAttributeSet
        {
            public string name { get; set; }
            public string description { get; set; }
            public string mandatorytype { get; set; }
            public bool IsGuaranteeDate { get; set; }
            public bool IsGuaranteeDateMandatory { get; set; }
            public string lotvalue { get; set; }
            public string serialvalue { get; set; }
            public bool islotcheck { get; set; }
            public bool isserialcheck { get; set; }
            public int ID { get; set; }
        }
        public class VA005_SerialCtr
        {
            public string name { get; set; }
            public int startno { get; set; }
            public int currentnext { get; set; }
            public int incrementno { get; set; }
            public string prefix { get; set; }
            public string sufix { get; set; }
            public bool IsLot { get; set; }
        }
        public class VA005_AddNewAttribute
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int M_AttributeSearch_ID { get; set; }
        }
        public class VA005_AddNewAttributeType
        {
            public string Value { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int M_Attribute_ID { get; set; }
        }
        public class VA005_GetLoadAttributeName
        {
            public string name { get; set; }
            public int id { get; set; }
        }
        public class VA005_GetAttributeNameByIcon
        {
            public string name { get; set; }
            public int id { get; set; }
        }



        //=================================================================================================================




        public class VA005_TreeStructure
        {
            public int id { get; set; }
            public string text { get; set; }
            public int NodeID { get; set; }
            public string color { get; set; }
            public string bColor { get; set; }
            public string ImageSource { get; set; }
            public string Image2 { get; set; }
            public string Image3 { get; set; }
            public string visibility { get; set; }
            public string ShowInfo { get; set; }
            public string Type { get; set; }
            public string UID { get; set; }
            public int NID { get; set; }
            public bool expanded { get; set; }
            public string padding { get; set; }
            public string margin { get; set; }
            public string checkbox { get; set; }
            public int ParentID { get; set; }
            public List<VA005_TreeStructure> items { get; set; }
            public string classforgetnod { get; set; }
            public string zindex { get; set; }


            //public int parentid { get; set; }
        }

        public class VA005_AttributeSet
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string MType { get; set; }
            public bool IsActive { get; set; }
            public bool IsExpiryDate { get; set; }
            public bool IsMExpiryDate { get; set; }
            public bool IsLot { get; set; }
            public bool IsSerial { get; set; }
            public int LotORSerialID { get; set; }
            public List<VA005_KeyVal> Lots { get; set; }
        }

        public class VA005_LotCtrl
        {
            public string Name { get; set; }
            public string StartNo { get; set; }
            public string CurrentNext { get; set; }
            public string Increment { get; set; }
            public string prefix { get; set; }
            public string Suffix { get; set; }
            public bool IsLot { get; set; }
        }

        public class VA005_KeyVal
        {
            public string Name { get; set; }
            public string ID { get; set; }
        }


        public class VA005_DeleteAttributeSetValue
        {
            public int attributesetdelID { get; set; }
        }

        public class VA005_DeleteAttributeFromData
        {
            public int deleteattributefromdataID { get; set; }
        }


        public class AttributeAppend
        {
            public int m_attribute_id { get; set; }
            public string name { get; set; }
            public string isactive { get; set; }
        }
        public class AttributeValue
        {
            public int m_attributevalue_id { get; set; }
            public string name { get; set; }
            public int m_attribute_id { get; set; }
        }
        public class EditAttribute
        {
            public int ad_window_id { get; set; }
        }

        public class AttributeData
        {
            public string name { get; set; }
            public string description { get; set; }
            public string mandatorytype { get; set; }
            public string isguaranteedate { get; set; }
            public string isguaranteedatemandatory { get; set; }
            public int m_lotctl_id { get; set; }
            public int m_sernoctl_id { get; set; }
            public string islot { get; set; }
            public string isserno { get; set; }
        }
        public class AttributeUseData
        {
            public string Name { get; set; }
        }
}
