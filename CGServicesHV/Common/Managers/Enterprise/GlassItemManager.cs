using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using CGServicesHV.Common.Types.E1;
using CGServicesHV.Common.Types.Enterprise;
using CGServicesHV.Common.Types.Response;
using CGServicesHV.Common.Utilities;
using IBM.Data.DB2.iSeries;

namespace CGServicesHV.Common.Managers.Enterprise
{
    public class GlassItemManager
    {
        private readonly CustomerManager _customerManager;
        private readonly ItemAttributeManager _itemAttributesManager;
        private readonly UdcItemManager _udcItemManager;
        private readonly FlexItemManager _flexItemManager;

        public GlassItemManager(BusinessSystemSession e1Session)
        {
            var hash = new Dictionary<string, iDB2Command>();
            hash.Add("GlassItemManager.GetItem", GetItem());
            foreach (var keyName in hash.Keys)
            {
                if (!e1Session.DbCommands.ContainsKey(keyName))
                {
                    e1Session.DbCommands.Add(keyName, hash[keyName]);
                }
            }
            // _itemManager = new ItemManager(e1Session);
            _itemAttributesManager = new ItemAttributeManager(e1Session);
            _customerManager = new CustomerManager(e1Session);
            _udcItemManager = new UdcItemManager(e1Session);
            _flexItemManager = new FlexItemManager(e1Session);
        }

        /********************************************************************************
        *  Glass Item
        *******************************************************************************/
        public iDB2Command GetItem()
        {
            var cmd = new iDB2Command
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT 
                        COALESCE(IBMCU, '00') AS IBMCU,
                        COALESCE(IBITM, IMITM) AS IBITM,
                        COALESCE(IBGLPT, IMGLPT) AS IBGLPT,
                        COALESCE(IBSTKT, IMSTKT) AS IBSTKT,
                        COALESCE(IBSRP4, IMSRP4) AS IBSRP4,
                        DRDL01,
                        IMDSC1,
                        IMDSC2               
                    FROM 
                        F4101
                        LEFT OUTER JOIN F4102
                        ON  IMITM = IBITM AND
                            IBMCU = ?
                        LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 UD1
                        ON  UD1.DRSY = '41' AND
                            UD1.DRRT = 'S4' AND
                            TRIM(UD1.DRKY) = TRIM(COALESCE(IBSRP4, IMSRP4))
                    WHERE 
                        IMITM = ?"
            };
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ItemNum"].Precision = 8;
            cmd.Parameters["ItemNum"].Scale = 0;
            return cmd;
        }

        public GlassItem BuildGlassItem(BusinessSystemSession e1Session, decimal itemNum, string plantNum, decimal documentNum = 0, string documentType = null, decimal documentLine = 0, string companyCode = null, ItemAttributeCache cache = null, SqlDmlResponse sqlResponse = null)
        {
            var item = new GlassItem();
            try
            {
                var cmd = (e1Session.DbCommands.ContainsKey("GlassItemManager.GetItem")) ? e1Session.DbCommands["GlassItemManager.GetItem"] : GetItem();
                cmd.Parameters["PlantNum"].Value = plantNum.PadLeft(12);
                cmd.Parameters["ItemNum"].Value = itemNum;
                cmd.Connection = e1Session.DbConnection;
                if (sqlResponse == null) sqlResponse = new SqlDmlResponse();
                sqlResponse.Init(cmd);
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        item.PlantNum = plantNum;
                        item.ItemNum = Data.CastDbNull.To<decimal>(reader["IBITM"], 0);
                        item.StockingType = Data.CastDbNull.To(reader["IBSTKT"], string.Empty).Trim();
                        item.GLCategory = Data.CastDbNull.To(reader["IBGLPT"], string.Empty).Trim();
                        item.Description = Data.CastDbNull.To(reader["IMDSC1"], string.Empty).Trim();
                        item.Construction = Data.CastDbNull.To(reader["IMDSC2"], string.Empty).Trim();
                        item.UnitType = Data.CastDbNull.To(reader["IBSRP4"], string.Empty).Trim();
                        item.UnitTypeDesc = Data.CastDbNull.To(reader["DRDL01"], string.Empty).Trim();

                        //var attributes = _itemAttributesManager.GetItemAttributes(e1Session, itemNum, documentNum, documentType, documentLine, companyCode, cache);
                        //item.ItemAttributes = attributes;

                        //// Customer
                        //var customerNum = attributes.GetDecimalAttributeValue("AN8DR");
                        //item.Customer = _customerManager.GetCustomer(e1Session, customerNum);
                        //item.CustomerItemReference = _customerManager.GetCustomerItemReference(e1Session, customerNum, itemNum);

                        //// Enrich
                        //if (attributes.Count > 0)
                        //{
                        //    EnrichItem(e1Session, item, attributes);
                        //}
                    }

                    reader.Close();
                    sqlResponse.RowsAffected = 1;
                    sqlResponse.EndTime = DateTime.Now.ToLocalTime();
                    sqlResponse.ElapsedSeconds = Convert.ToDecimal((sqlResponse.EndTime - sqlResponse.StartTime).TotalSeconds);
                    sqlResponse.Name = "BuildGlassItem";
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return item;
        }

        public GlassItem GetGlassItem(BusinessSystemSession e1Session, decimal itemNum, string plantCode, decimal documentNum = 0, string documentType = null, decimal documentLine = 0, string companyCode = null, ItemAttributeCache cache = null)
        {
            var item = new GlassItem();
            try
            {
                var plantNum = PlantManager.GetPlant(plantCode).PlantCode;
                var cmd = (e1Session.DbCommands.ContainsKey("GlassItemManager.GetItem"))
                    ? e1Session.DbCommands["GlassItemManager.GetItem"]
                    : GetItem();
                cmd.Parameters[0].Value = plantNum.PadLeft(12);
                cmd.Parameters[1].Value = itemNum;
                cmd.Connection = e1Session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        item.PlantNum = plantNum;
                        item.ItemNum = Data.CastDbNull.To<decimal>(reader["IBITM"], 0);
                        item.StockingType = Data.CastDbNull.To(reader["IBSTKT"], string.Empty).Trim();
                        item.GLCategory = Data.CastDbNull.To(reader["IBGLPT"], string.Empty).Trim();
                        item.Description = Data.CastDbNull.To(reader["IMDSC1"], string.Empty).Trim();
                        item.Construction = Data.CastDbNull.To(reader["IMDSC2"], string.Empty).Trim();
                        item.UnitType = Data.CastDbNull.To(reader["IBSRP4"], string.Empty).Trim();
                        item.UnitTypeDesc = Data.CastDbNull.To(reader["DRDL01"], string.Empty).Trim();

                        var attributes = _itemAttributesManager.GetItemAttributes(e1Session, itemNum, documentNum, documentType, documentLine, companyCode);
                        item.ItemAttributes = attributes;

                        // Customer
                        var customerNum = attributes.GetDecimalAttributeValue("AN8DR");
                        item.Customer = _customerManager.GetCustomer(e1Session, customerNum);
                        item.CustomerItemReference = _customerManager.GetCustomerItemReference(e1Session, customerNum, itemNum);

                        // Enrich
                        if (attributes.Count > 0)
                        {
                            EnrichItem(e1Session, item, attributes);
                        }

                        // Flex
                        var flexDescList = _flexItemManager.GetDescription(e1Session, "P5842300", attributes);
                        item.FlexItemDesc1 = flexDescList[0].Trim();
                        item.FlexItemDesc2 = flexDescList[1].Trim();
                        if (flexDescList.Count >= 3)
                        {
                            item.FlexItemDesc3 = flexDescList[2].Trim();
                        }

                        if (flexDescList.Count >= 4)
                        {
                            item.FlexItemDesc4 = flexDescList[3].Trim();
                        }

                        if (flexDescList.Count >= 5)
                        {
                            item.FlexItemDesc5 = flexDescList[4].Trim();
                        }

                        if (flexDescList.Count >= 6)
                        {
                            item.FlexItemDesc6 = flexDescList[5].Trim();
                        }

                        if (flexDescList.Count >= 7)
                        {
                            item.FlexItemDesc7 = flexDescList[6].Trim();
                        }

                        if (flexDescList.Count >= 8)
                        {
                            item.FlexItemDesc8 = flexDescList[7].Trim();
                        }

                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return item;
        }

        /********************************************************************************
        *  Discrete Item
        *******************************************************************************/
        public iDB2Command GetDiscreteItemsByPlant()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
                SELECT 
                    IBMCU,
                    IBITM,
                    IBGLPT,
                    IBSTKT,
                    IBSRP4,
                    DRDL01,
                    IMDSC1,
                    IMDSC2               
                FROM 
                    F4102
                    INNER JOIN F4101
                    ON  IMITM = IBITM
                    LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 UD1
                    ON  UD1.DRSY = '41' AND
                        UD1.DRRT = 'S4' AND
                        TRIM(UD1.DRKY) = TRIM(IBSRP4)
                WHERE 
                    IBMCU = ? AND
                    IBGLPT IN ('GLAS','GLAF','GLAX','GLAZ','COMA','GLAC') AND
                    IBSTKT != 'C'";
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            return cmd;
        }

        public iDB2Command GetDiscreteItem()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
                SELECT 
                    IBMCU,
                    IBITM,
                    IBGLPT,
                    IBSTKT,
                    IBSRP4,
                    DRDL01,
                    IMDSC1,
                    IMDSC2               
                FROM 
                    F4102
                    INNER JOIN F4101
                    ON  IMITM = IBITM
                    LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 UD1
                    ON  UD1.DRSY = '41' AND
                        UD1.DRRT = 'S4' AND
                        TRIM(UD1.DRKY) = TRIM(IBSRP4)
                WHERE 
                    IBMCU = ? AND
                    IBGLPT IN ('GLAS','GLAF','GLAX','GLAZ','COMA','GLAC') AND
                    IBITM = ? AND
                    IBSTKT != 'C'";
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("ItemNum", iDB2DbType.iDB2Decimal));
            cmd.Parameters["ItemNum"].Precision = 8;
            cmd.Parameters["ItemNum"].Scale = 0;
            return cmd;
        }

        public iDB2Command GetDiscreteItemsByTimespan()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $@"
                SELECT 
                    IBMCU,
                    IBITM,
                    IBGLPT,
                    IBSTKT,
                    IBSRP4,
                    DRDL01,
                    IMDSC1,
                    IMDSC2               
                FROM 
                    F4102
                    INNER JOIN F4101
                    ON  IMITM = IBITM
                    LEFT OUTER JOIN {ConfigurationManager.AppSettings["ControlLib"]}.F0005 UD1
                    ON  UD1.DRSY = '41' AND
                        UD1.DRRT = 'S4' AND
                        TRIM(UD1.DRKY) = TRIM(IBSRP4)
                WHERE 
                    IBSTKT != 'C' AND
                    IBGLPT IN ('GLAS','GLAF','GLAX','GLAZ','COMA','GLAC') AND
                    IBMCU = ? AND
                    EXISTS (
                            SELECT 
                                1 
                            FROM 
                                F580002 
                            WHERE 
				                IAITM = IMITM AND 
                                (
                                    (IAUPMJ = ? AND IATDAY >= ?) OR
                                    (IAUPMJ >= ? AND IAUPMJ <= ?)
							    )
						    )";
            cmd.Parameters.Add(new iDB2Parameter("PlantNum", iDB2DbType.iDB2Graphic, 12));
            cmd.Parameters.Add(new iDB2Parameter("UPMJ1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["UPMJ1"].Precision = 6;
            cmd.Parameters["UPMJ1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("TDAY1", iDB2DbType.iDB2Decimal));
            cmd.Parameters["TDAY1"].Precision = 6;
            cmd.Parameters["TDAY1"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("UPMJ2", iDB2DbType.iDB2Decimal));
            cmd.Parameters["UPMJ2"].Precision = 6;
            cmd.Parameters["UPMJ2"].Scale = 0;
            cmd.Parameters.Add(new iDB2Parameter("UPMJ3", iDB2DbType.iDB2Decimal));
            cmd.Parameters["UPMJ3"].Precision = 6;
            cmd.Parameters["UPMJ3"].Scale = 0;
            return cmd;
        }

        public GlassItems GetDiscreteItems(BusinessSystemSession e1Session, string plantCode, Decimal itemNum = 0, TimeSpan? timeSpan = null)
        {
            var items = new GlassItems();
            try
            {
                var plantNum = PlantManager.GetPlant(plantCode).PlantCode;
                iDB2Command cmd = new iDB2Command();

                var ets = new E1TimeSpan();
                if (timeSpan != null && timeSpan.Value.TotalDays > 0)
                {
                    ets.SetTimeSpan(timeSpan.Value);
                    cmd = (e1Session.DbCommands.ContainsKey("GlassItemManager.GetDiscreteItemsByTimespan"))
                        ? e1Session.DbCommands["GlassItemManager.GetDiscreteItemsByTimespan"]
                        : GetDiscreteItemsByTimespan();
                    cmd.Parameters[0].Value = plantNum.PadLeft(12);
                    cmd.Parameters[1].Value = ets.DateComponent1;
                    cmd.Parameters[2].Value = ets.TimeComponent1;
                    cmd.Parameters[3].Value = ets.DateComponent2;
                    cmd.Parameters[4].Value = ets.DateComponent3;
                }
                else
                {
                    if (itemNum > 0)
                    {
                        cmd = (e1Session.DbCommands.ContainsKey("GlassItemManager.GetDiscreteItem"))
                            ? e1Session.DbCommands["GlassItemManager.GetDiscreteItem"]
                            : GetDiscreteItem();
                        cmd.Parameters[0].Value = plantNum.PadLeft(12);
                        cmd.Parameters[1].Value = itemNum;

                    }
                    else
                    {
                        cmd = (e1Session.DbCommands.ContainsKey("GlassItemManager.GetDiscreteItemsByPlant"))
                            ? e1Session.DbCommands["GlassItemManager.GetDiscreteItemsByPlant"]
                            : GetDiscreteItemsByPlant();
                        cmd.Parameters[0].Value = plantNum.PadLeft(12);
                    }
                }

                cmd.Connection = e1Session.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var item = new GlassItem();
                        item.PlantNum = plantNum;
                        item.ItemNum = Data.CastDbNull.To<Decimal>(reader["IBITM"], 0);
                        item.StockingType = Data.CastDbNull.To<String>(reader["IBSTKT"], String.Empty).Trim();
                        item.GLCategory = Data.CastDbNull.To<String>(reader["IBGLPT"], String.Empty).Trim();
                        item.Description = Data.CastDbNull.To<String>(reader["IMDSC1"], String.Empty).Trim();
                        item.Construction = Data.CastDbNull.To<String>(reader["IMDSC2"], String.Empty).Trim();
                        item.UnitType = Data.CastDbNull.To<String>(reader["IBSRP4"], String.Empty).Trim();
                        item.UnitTypeDesc = Data.CastDbNull.To<String>(reader["DRDL01"], String.Empty).Trim();
                        ItemAttributes attributes = _itemAttributesManager.GetItemAttributes(e1Session, item.ItemNum, 0, "", 0, "");
                        item.ItemAttributes = attributes;

                        // Customer
                        decimal customerNum = attributes.GetDecimalAttributeValue("AN8DR");
                        item.Customer = _customerManager.GetCustomer(e1Session, customerNum);
                        item.CustomerItemReference = _customerManager.GetCustomerItemReference(e1Session, customerNum, itemNum);

                        // Enrich
                        if (attributes.Count > 0)
                        {
                            EnrichItem(e1Session, item, attributes);
                        }

                        items.Add(item);
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return items;
        }


        /********************************************************************************
        * Segments
        *******************************************************************************/
        public void EnrichItem(BusinessSystemSession e1Session, GlassItem item, ItemAttributes segments, SqlDmlResponse sqlResponse = null)
        {
            try
            {
                //var unit = new GlassUnit();
                item.OverallThickness = segments.GetDecimalAttributeValue("Z5850865");
                item.LitesPerRack = segments.GetDecimalAttributeValue("Z58LPR");
                item.UnitTreatment = segments.GetStringAttributeValue("Z5800513");
                item.UnitTreatmentDesc = segments.GetStringAttributeDesc1("Z5800513");
                //item.UnitType = segments.GetStringAttributeValue("Z5840275");
                //item.UnitTypeDesc = segments.GetStringAttributeDesc1("Z5840275");
                item.RackRows = segments.GetDecimalAttributeValue("Z58ROW");
                item.RackTiers = segments.GetDecimalAttributeValue("Z58STK");
                item.RackOrientationVH = segments.GetStringAttributeValue("Z58RPRVH");
                item.RackOrientationIO = segments.GetStringAttributeValue("Z58RTNIO");
                item.ProductType = segments.GetStringAttributeDesc1("Z58PT");
                item.CardinalProductType = segments.GetStringAttributeDesc1("Z5840310");
                item.CornerClipping = segments.GetStringAttributeDesc1("Z5800550");
                item.VolumeIndicator = segments.GetStringAttributeValue("Z5840460");
                item.BatchShiftLotNum = segments.GetStringAttributeValue("Z5895856");
                item.VerticalSashIntrusion = segments.GetDecimalAttributeValue("Z58IGSIV");
                item.HorizontalSashIntrusion = segments.GetDecimalAttributeValue("Z58IGSIH");
                item.NarrowMarginCode = segments.GetStringAttributeValue("Z58IGNMC");
                item.NarrowMarginDesc = segments.GetStringAttributeDesc1("Z58IGNMC");
                item.EnergyPanel = segments.GetStringAttributeValue("Z5801630");
                item.EnergyPanelDesc = segments.GetStringAttributeDesc1("Z5801630");
                item.IGCaps = segments.GetStringAttributeValue("Z5801635");
                item.IGCapsDesc = segments.GetStringAttributeDesc1("Z5801635");
                item.TotalWeight = segments.GetDecimalAttributeValue("Z58GLWT") /
                                   ((segments.GetDecimalAttributeValue("Z5840360") == 0)
                                        ? 1
                                        : segments.GetDecimalAttributeValue("Z5840360"));

                item.CustomerPartNum1 = segments.GetStringAttributeValue("Z58CP2");
                item.CustomerPartNum2 = segments.GetStringAttributeValue("Z58CP3");
                item.CustomerPartNum3 = segments.GetStringAttributeValue("Z58CP4");
                item.EndPickItemNum = segments.GetDecimalAttributeValue("Z5880230");
                //if (item.EndPickItemNum > 0)
                //{
                //    item.BasePickLocation = GetPickLocation(e1Session, item.PlantNum, item.EndPickItemNum, "P");
                //}

                // Customer
                item.SoldToCustomerNum = segments.GetDecimalAttributeValue("AN8DR");
                item.OriginalSoldToCustomerNum = segments.GetDecimalAttributeValue("Z58CTL");
                item.ShipToCustomerNum = segments.GetDecimalAttributeValue("SHAN");
                item.OriginalShipToCustomerNum = segments.GetDecimalAttributeValue("Z5840425");

                // Shape
                var unitShape = new Shape();
                unitShape.ShapeNum = segments.GetDecimalAttributeValue("Z58SHAPE");
                unitShape.ActualSqft = segments.GetDecimalAttributeValue("Z58USF");
                unitShape.BillableSqft = segments.GetDecimalAttributeValue("Z58BSF");
                unitShape.BlockSizePerimeter = segments.GetDecimalAttributeValue("Z5850970");
                unitShape.BlockSizeHeight = segments.GetDecimalAttributeValue("Z5800536");
                unitShape.BlockSizeWidth = segments.GetDecimalAttributeValue("Z5800537");
                unitShape.BlockSizeShort = segments.GetDecimalAttributeValue("Z5800538");
                unitShape.BlockSizeLong = segments.GetDecimalAttributeValue("Z5800539");

                var baseDimension = new Dimension();
                baseDimension.DecimalValue = segments.GetDecimalAttributeValue("Z581BDEC");
                baseDimension.MMValue = segments.GetDecimalAttributeValue("Z581BMIM");
                baseDimension.Base = segments.GetDecimalAttributeValue("Z58L1BDM");
                baseDimension.Numerator = segments.GetDecimalAttributeValue("Z58L1BNM");
                baseDimension.Denominator = segments.GetDecimalAttributeValue("Z58L1BDN");
                baseDimension.Description = baseDimension.GetDescription();
                unitShape.BaseDimension = baseDimension;

                var leftDimension = new Dimension();
                leftDimension.DecimalValue = segments.GetDecimalAttributeValue("Z581LDEC");
                leftDimension.MMValue = segments.GetDecimalAttributeValue("Z581LMIM");
                leftDimension.Base = segments.GetDecimalAttributeValue("Z58L1LDM");
                leftDimension.Numerator = segments.GetDecimalAttributeValue("Z58L1LNM");
                leftDimension.Denominator = segments.GetDecimalAttributeValue("Z58L1LDN");
                leftDimension.Description = leftDimension.GetDescription();
                unitShape.LeftDimension = leftDimension;

                var rightDimension = new Dimension();
                rightDimension.DecimalValue = segments.GetDecimalAttributeValue("Z581RDEC");
                rightDimension.MMValue = segments.GetDecimalAttributeValue("Z581RMIM");
                rightDimension.Base = segments.GetDecimalAttributeValue("Z58L1RDM");
                rightDimension.Numerator = segments.GetDecimalAttributeValue("Z58L1RNM");
                rightDimension.Denominator = segments.GetDecimalAttributeValue("Z58L1RDN");
                rightDimension.Description = rightDimension.GetDescription();
                unitShape.RightDimension = rightDimension;

                var topDimension = new Dimension();
                topDimension.DecimalValue = segments.GetDecimalAttributeValue("Z581TDEC");
                topDimension.MMValue = segments.GetDecimalAttributeValue("Z581TMIM");
                topDimension.Base = segments.GetDecimalAttributeValue("Z58L1TDM");
                topDimension.Numerator = segments.GetDecimalAttributeValue("Z58L1TNM");
                topDimension.Denominator = segments.GetDecimalAttributeValue("Z58L1TDN");
                topDimension.Description = topDimension.GetDescription();
                unitShape.TopDimension = topDimension;

                var s1Dimension = new Dimension();
                s1Dimension.DecimalValue = segments.GetDecimalAttributeValue("Z5811DEC");
                s1Dimension.MMValue = segments.GetDecimalAttributeValue("Z5811MIM");
                s1Dimension.Base = segments.GetDecimalAttributeValue("Z58L11DM");
                s1Dimension.Numerator = segments.GetDecimalAttributeValue("Z58L11NM");
                s1Dimension.Denominator = segments.GetDecimalAttributeValue("Z58L11DN");
                s1Dimension.Description = s1Dimension.GetDescription();
                unitShape.Side1Dimension = s1Dimension;

                var s2Dimension = new Dimension();
                s2Dimension.DecimalValue = segments.GetDecimalAttributeValue("Z5812DEC");
                s2Dimension.MMValue = segments.GetDecimalAttributeValue("Z5812MIM");
                s2Dimension.Base = segments.GetDecimalAttributeValue("Z58L12DM");
                s2Dimension.Numerator = segments.GetDecimalAttributeValue("Z58L12NM");
                s2Dimension.Denominator = segments.GetDecimalAttributeValue("Z58L12DN");
                s2Dimension.Description = s2Dimension.GetDescription();
                unitShape.Side2Dimension = s2Dimension;

                item.Shape = unitShape;

                // Lites
                var numLites = 0;
                switch (item.UnitType)
                {
                    case "1":
                        numLites = 1;
                        break;
                    case "2":
                        numLites = 2;
                        break;
                    case "3":
                        numLites = 3;
                        break;
                    default:
                        numLites = 1;
                        break;
                }
                var lites = new Lites();
                item.NumLites = numLites;

                // Lite 1
                var lite1 = new Lite();
                lite1.Position = 1;
                lite1.RoutingCode = segments.GetStringAttributeValue("Z58L1RTG");
                lite1.RoutingDesc = segments.GetStringAttributeDesc1("Z58L1RTG");
                lite1.RawPickItemNum = segments.GetDecimalAttributeValue("Z5851551");
                //if (lite1.RawPickItemNum > 0)
                //{
                //    lite1.RawPickLocation = GetPickLocation(e1Session, item.PlantNum, lite1.RawPickItemNum, "R");
                //}

                var gt1 = new GlassType();
                gt1.Thickness = segments.GetDecimalAttributeValue("Z58L1TK");

                gt1.HeatTreatment = segments.GetStringAttributeValue("Z58L1HT", "A");
                gt1.HeatTreatmentCode = segments.GetStringAttributeDesc3("Z58L1HT");
                //gt1.HeatTreatmentDesc = segments.GetStringAttributeDesc1("Z58L1HT");
                gt1.Color = segments.GetStringAttributeValue("Z58L1CL", "");
                gt1.ColorCode = _udcItemManager.GetUdcItem(e1Session, "57", "CL", gt1.Color).SpecialHandlingCode;
                //gt1.ColorDesc = _udcItemManager.GetUdcItem(e1Session, "57", "CL", gt1.Color).Description1;
                gt1.Surface1Coating = segments.GetStringAttributeValue("Z58L11C", string.Empty);
                gt1.Surface1CoatingCode = segments.GetStringAttributeDesc3("Z58L11C");
                //gt1.Surface1CoatingDesc = segments.GetStringAttributeDesc1("Z58L11C");
                gt1.Surface2Coating = segments.GetStringAttributeValue("Z58L12C", string.Empty);
                gt1.Surface2CoatingCode = segments.GetStringAttributeDesc3("Z58L12C");
                //gt1.Surface2CoatingDesc = segments.GetStringAttributeDesc1("Z58L12C");
                gt1.PatternTexture = segments.GetStringAttributeValue("Z58L1PX", string.Empty);
                gt1.PatternTextureCode = segments.GetStringAttributeDesc3("Z58L1PX");
                //gt1.PatternTextureDesc = segments.GetStringAttributeDesc1("Z58L1PX");
                gt1.PatternTextureSrf = segments.GetDecimalAttributeValue("Z58L1PTS");
                //gt1.PatternTextureSrfDesc = segments.GetStringAttributeDesc1("Z58L1PTS");
                gt1.LamiProductType = segments.GetStringAttributeValue("Z58L1LPT", string.Empty);
                gt1.LamiProductTypeCode = segments.GetStringAttributeDesc3("Z58L1LPT");
                //gt1.LamiProductTypeDesc = segments.GetStringAttributeDesc1("Z58L1LPT");
                //gt1.LamiColorSrf = segments.GetDecimalAttributeValue("Z58L1LCS");
                //gt1.LamiColorSrfDesc = segments.GetStringAttributeDesc1("Z58L1LCS");

                gt1.LamiMixedTreatment = segments.GetStringAttributeValue("Z5801865");
                gt1.LamiPatternSurface = segments.GetStringAttributeValue("Z58L1LPS");
                gt1.LamiCoatingSurface = segments.GetStringAttributeValue("Z58L1LXS");
                gt1.LamiColorSurface = segments.GetStringAttributeValue("Z5801890");
                gt1.LamiThickness1 = segments.GetDecimalAttributeValue("Z5801905");
                gt1.LamiInnerLayerThickness = segments.GetDecimalAttributeValue("Z5801906");
                gt1.LamiThickness2 = segments.GetDecimalAttributeValue("Z5801907");

                gt1.TempLogo = segments.GetStringAttributeValue("Z58L1TL");
                gt1.CustomerSupplied = (segments.GetStringAttributeValue("Z5840450") != null) &&
                                       segments.GetStringAttributeValue("Z5840450").Contains("1");
                lite1.GlassType = gt1;

                lite1.QualityLevel = segments.GetStringAttributeValue("Z58L1PQL");
                lite1.QualityLevelDesc = segments.GetStringAttributeDesc1("Z58L1PQL");

                lite1.VGrooveType = segments.GetStringAttributeValue("Z58L1VG");
                lite1.VGrooveTypeDesc = segments.GetStringAttributeDesc1("Z58L1VG");
                lite1.VGrooveStyle = segments.GetStringAttributeValue("Z58L1VGT");
                lite1.VGrooveStyleDesc = segments.GetStringAttributeDesc1("Z58L1VGT");
                lite1.VGrooveSurface = segments.GetDecimalAttributeValue("Z58L1VGS");
                lite1.VGrooveDrawing = segments.GetStringAttributeValue("Z58L1VGD");
                lite1.VGrooveHorzBars = segments.GetDecimalAttributeValue("Z5801180");
                lite1.VGrooveVertBars = segments.GetDecimalAttributeValue("Z5801190");
                lite1.VGrooveHighOpen = segments.GetDecimalAttributeValue("Z58L1VGH");
                lite1.VGrooveWideOpen = segments.GetDecimalAttributeValue("Z58L1VGW");
                lite1.VGrooveTotalOpen = segments.GetDecimalAttributeValue("Z58L1VGO");
                lite1.VGrooveBevel = segments.GetStringAttributeValue("Z58L1BL");
                lite1.VGrooveVertOffset = segments.GetDecimalAttributeValue("Z5801295");
                lite1.VGrooveHorzOffset = segments.GetDecimalAttributeValue("Z5801300");
                lite1.VGrooveVertStartStop = segments.GetStringAttributeValue("Z5801296");
                lite1.VGrooveHorzStartStop = segments.GetStringAttributeValue("Z5801301");

                lite1.NotchHeight = segments.GetDecimalAttributeValue("Z5801481");
                lite1.NotchLocation = segments.GetStringAttributeValue("Z5801480");
                lite1.NotchWidth = segments.GetDecimalAttributeValue("Z5801482");

                lite1.HolesDrawing = segments.GetStringAttributeValue("Z58L1HL");
                lite1.HolesNum = segments.GetDecimalAttributeValue("Z581365");

                lite1.EdgeWork = segments.GetStringAttributeValue("Z58L1EW");
                lite1.EdgeWorkDesc = segments.GetStringAttributeDesc1("Z58L1EW");
                lite1.EdgeDeleteWidthCode = segments.GetStringAttributeValue("Z5805680");
                lite1.EdgeDeleteWidthValue = segments.GetStringAttributeDesc3("Z5805680") == string.Empty
                                                 ? 0
                                                 : Convert.ToDecimal(segments.GetStringAttributeDesc3("Z5805680"));

                lite1.SilkScreenDrawing = segments.GetStringAttributeValue("Z58L1SD");
                lite1.SilkScreenSurface = segments.GetDecimalAttributeValue("Z58L1SF");

                lite1.EtchingDrawing = segments.GetStringAttributeValue("Z5801895");

                lites.Add(lite1);

                // Lite 2
                if (item.NumLites > 1)
                {
                    var lite2 = new Lite();
                    lite2.Position = 2;
                    lite2.RoutingCode = segments.GetStringAttributeValue("Z58L2RTG");
                    lite2.RoutingDesc = segments.GetStringAttributeDesc1("Z58L2RTG");
                    lite2.RawPickItemNum = segments.GetDecimalAttributeValue("Z5851552");
                    //if (lite2.RawPickItemNum > 0)
                    //{
                    //    lite2.RawPickLocation = GetPickLocation(e1Session, item.PlantNum, lite2.RawPickItemNum, "R");
                    //}
                    var gt2 = new GlassType();
                    gt2.Thickness = segments.GetDecimalAttributeValue("Z58L2TK");
                    gt2.HeatTreatment = segments.GetStringAttributeValue("Z58L2HT", "A");
                    gt2.HeatTreatmentCode = segments.GetStringAttributeDesc3("Z58L2HT");
                    gt2.Color = segments.GetStringAttributeValue("Z58L2CL", "");
                    gt2.ColorCode = _udcItemManager.GetUdcItem(e1Session, "57", "CL", gt2.Color).SpecialHandlingCode;
                    gt2.Surface1Coating = segments.GetStringAttributeValue("Z58L24C", string.Empty);
                    gt2.Surface1CoatingCode = segments.GetStringAttributeDesc3("Z58L24C");
                    gt2.Surface2Coating = segments.GetStringAttributeValue("Z58L23C", string.Empty);
                    gt2.Surface2CoatingCode = segments.GetStringAttributeDesc3("Z58L23C");
                    gt2.PatternTexture = segments.GetStringAttributeValue("Z58L2PX", string.Empty);
                    gt2.PatternTextureCode = segments.GetStringAttributeDesc3("Z58L2PX");
                    gt2.PatternTextureSrf = segments.GetDecimalAttributeValue("Z58L2PTS");
                    gt2.LamiProductType = segments.GetStringAttributeValue("Z58L2LPT", string.Empty);
                    gt2.LamiProductTypeCode = segments.GetStringAttributeDesc3("Z58L2LPT");
                    gt2.LamiPatternSurface = segments.GetStringAttributeValue("Z58L2LPS");
                    gt2.LamiCoatingSurface = segments.GetStringAttributeValue("Z58L2LXS");
                    gt2.TempLogo = segments.GetStringAttributeValue("Z58L2TL");
                    gt2.CustomerSupplied = (segments.GetStringAttributeValue("Z5840450") != null) &&
                                           segments.GetStringAttributeValue("Z5840450").Contains("2");
                    lite2.GlassType = gt2;

                    lite2.VGrooveType = segments.GetStringAttributeValue("Z58L2VG");
                    lite2.VGrooveTypeDesc = segments.GetStringAttributeDesc1("Z58L2VG");
                    lite2.VGrooveStyle = segments.GetStringAttributeValue("Z58L2VGT");
                    lite2.VGrooveStyleDesc = segments.GetStringAttributeDesc1("Z58L2VGT");
                    lite2.VGrooveSurface = segments.GetDecimalAttributeValue("Z58L2VGS");
                    lite2.VGrooveDrawing = segments.GetStringAttributeValue("Z58L2VGD");
                    lite2.VGrooveHorzBars = segments.GetDecimalAttributeValue("Z5802830");
                    lite2.VGrooveVertBars = segments.GetDecimalAttributeValue("Z5802840");
                    lite2.VGrooveHighOpen = segments.GetDecimalAttributeValue("Z58L2VGH");
                    lite2.VGrooveWideOpen = segments.GetDecimalAttributeValue("Z58L2VGW");
                    lite2.VGrooveTotalOpen = segments.GetDecimalAttributeValue("Z58L2VGO");
                    lite2.VGrooveBevel = segments.GetStringAttributeValue("Z58L2BL");
                    lite2.VGrooveVertOffset = segments.GetDecimalAttributeValue("Z5801295");
                    lite2.VGrooveHorzOffset = segments.GetDecimalAttributeValue("Z5801300");
                    lite2.VGrooveVertStartStop = segments.GetStringAttributeValue("Z5801296");
                    lite2.VGrooveHorzStartStop = segments.GetStringAttributeValue("Z5801301");

                    lite2.NotchHeight = segments.GetDecimalAttributeValue("Z5801481");
                    lite2.NotchLocation = segments.GetStringAttributeValue("Z5801480");
                    lite2.NotchWidth = segments.GetDecimalAttributeValue("Z5801482");

                    lite2.HolesDrawing = segments.GetStringAttributeValue("Z58L2HL");
                    lite2.HolesNum = segments.GetDecimalAttributeValue("Z5803005");

                    lite2.EdgeWork = segments.GetStringAttributeValue("Z58L2EW");
                    lite2.EdgeDeleteWidthCode = segments.GetStringAttributeValue("Z5805680");
                    lite2.EdgeDeleteWidthValue = segments.GetStringAttributeDesc3("Z5805680") == string.Empty
                                                     ? 0
                                                     : Convert.ToDecimal(segments.GetStringAttributeDesc3("Z5805680"));

                    lite2.SilkScreenDrawing = segments.GetStringAttributeValue("Z58L2SD");
                    lite2.SilkScreenSurface = segments.GetDecimalAttributeValue("Z58L2SF");

                    lite2.EtchingDrawing = segments.GetStringAttributeValue("Z5803515");

                    lites.Add(lite2);
                }

                // Lite 3
                if (item.NumLites > 2)
                {
                    var lite3 = new Lite();
                    lite3.Position = 3;
                    lite3.RoutingCode = segments.GetStringAttributeValue("Z58L3RTG");
                    lite3.RoutingDesc = segments.GetStringAttributeDesc1("Z58L3RTG");
                    lite3.RawPickItemNum = segments.GetDecimalAttributeValue("Z5851553");
                    //if (lite3.RawPickItemNum > 0)
                    //{
                    //    lite3.RawPickLocation = GetPickLocation(e1Session, item.PlantNum, lite3.RawPickItemNum, "R");
                    //}

                    var gt3 = new GlassType();
                    gt3.Thickness = segments.GetDecimalAttributeValue("Z58L3TK");
                    gt3.HeatTreatment = segments.GetStringAttributeValue("Z58L3HT", "A");
                    gt3.HeatTreatmentCode = segments.GetStringAttributeDesc3("Z58L3HT");
                    gt3.Color = segments.GetStringAttributeValue("Z58L3CL", "");
                    gt3.ColorCode = _udcItemManager.GetUdcItem(e1Session, "57", "CL", gt3.Color).SpecialHandlingCode;
                    gt3.Surface1Coating = segments.GetStringAttributeValue("Z58L36C", string.Empty);
                    gt3.Surface1CoatingCode = segments.GetStringAttributeDesc3("Z58L36C");
                    gt3.Surface2Coating = segments.GetStringAttributeValue("Z58L35C", string.Empty);
                    gt3.Surface2CoatingCode = segments.GetStringAttributeDesc3("Z58L35C");
                    gt3.PatternTexture = segments.GetStringAttributeValue("Z58L3PX", string.Empty);
                    gt3.PatternTextureCode = segments.GetStringAttributeDesc3("Z58L3PX");
                    gt3.PatternTextureSrf = segments.GetDecimalAttributeValue("Z58L3PTS");
                    gt3.LamiProductType = segments.GetStringAttributeValue("Z58L3LPT", string.Empty);
                    gt3.LamiProductTypeCode = segments.GetStringAttributeDesc3("Z58L3LPT");
                    gt3.LamiPatternSurface = segments.GetStringAttributeValue("Z58L3LPS");
                    gt3.LamiCoatingSurface = segments.GetStringAttributeValue("Z58L3LXS");
                    gt3.TempLogo = segments.GetStringAttributeValue("Z58L3TL");
                    gt3.CustomerSupplied = (segments.GetStringAttributeValue("Z5840450") != null) &&
                                           segments.GetStringAttributeValue("Z5840450").Contains("3");
                    lite3.GlassType = gt3;

                    lite3.VGrooveType = segments.GetStringAttributeValue("Z58L3VG");
                    lite3.VGrooveTypeDesc = segments.GetStringAttributeDesc1("Z58L3VG");
                    lite3.VGrooveStyle = segments.GetStringAttributeValue("Z58L3VGT");
                    lite3.VGrooveStyleDesc = segments.GetStringAttributeDesc1("Z58L3VGT");
                    lite3.VGrooveSurface = segments.GetDecimalAttributeValue("Z58L3VGS");
                    lite3.VGrooveDrawing = segments.GetStringAttributeValue("Z58L3VGD");
                    lite3.VGrooveHorzBars = segments.GetDecimalAttributeValue("Z5804420");
                    lite3.VGrooveVertBars = segments.GetDecimalAttributeValue("Z5804430");
                    lite3.VGrooveHighOpen = segments.GetDecimalAttributeValue("Z58L3VGH");
                    lite3.VGrooveWideOpen = segments.GetDecimalAttributeValue("Z58L3VGW");
                    lite3.VGrooveTotalOpen = segments.GetDecimalAttributeValue("Z58L3VGO");
                    lite3.VGrooveBevel = segments.GetStringAttributeValue("Z58L3BL");
                    lite3.VGrooveVertOffset = segments.GetDecimalAttributeValue("Z5801295");
                    lite3.VGrooveHorzOffset = segments.GetDecimalAttributeValue("Z5801300");
                    lite3.VGrooveVertStartStop = segments.GetStringAttributeValue("Z5801296");
                    lite3.VGrooveHorzStartStop = segments.GetStringAttributeValue("Z5801301");

                    lite3.NotchHeight = segments.GetDecimalAttributeValue("Z5801481");
                    lite3.NotchLocation = segments.GetStringAttributeValue("Z5801480");
                    lite3.NotchWidth = segments.GetDecimalAttributeValue("Z5801482");

                    lite3.HolesDrawing = segments.GetStringAttributeValue("Z58L3HL");
                    lite3.HolesNum = segments.GetDecimalAttributeValue("Z5804595");

                    lite3.EdgeWork = segments.GetStringAttributeValue("Z58L3EW");
                    lite3.EdgeDeleteWidthCode = segments.GetStringAttributeValue("Z5805680");
                    lite3.EdgeDeleteWidthValue = segments.GetStringAttributeDesc3("Z5805680") == string.Empty
                                                     ? 0
                                                     : Convert.ToDecimal(segments.GetStringAttributeDesc3("Z5805680"));

                    lite3.SilkScreenDrawing = segments.GetStringAttributeValue("Z58L3SD");
                    lite3.SilkScreenSurface = segments.GetDecimalAttributeValue("Z58L3SF");

                    lite3.EtchingDrawing = segments.GetStringAttributeValue("Z5805105");

                    lites.Add(lite3);
                }
                item.Lites = lites;


                // Surfaces
                var surface1 = new Surface();
                surface1.PreserveColor = segments.GetStringAttributeValue("Z587566");
                surface1.PreserveCutback = segments.GetStringAttributeValue("Z587563");
                surface1.Opacifier = segments.GetStringAttributeValue("Z587572");
                surface1.OpacifierDesc = segments.GetStringAttributeDesc1("Z587572");
                surface1.ScoringWidth = segments.GetDecimalAttributeValue("Z587569");
                item.ExteriorSurface = surface1;

                var surface2 = new Surface();
                surface2.PreserveColor = segments.GetStringAttributeValue("Z587578");
                surface2.PreserveCutback = segments.GetStringAttributeValue("Z587575");
                surface2.Opacifier = segments.GetStringAttributeValue("Z587584");
                surface2.OpacifierDesc = segments.GetStringAttributeDesc1("Z587584");
                surface2.ScoringWidth = segments.GetDecimalAttributeValue("Z587581");
                item.InteriorSurface = surface2;

                // Logo
                var logo = new Logo();
                logo.AnsiLogo = segments.GetStringAttributeValue("Z58L1ALT");
                logo.XOffset = segments.GetDecimalAttributeValue("Z58L1LX");
                logo.YOffset = segments.GetDecimalAttributeValue("Z58L1LY");
                logo.NfrcLogo1 = segments.GetStringAttributeValue("Z58NFRC");
                logo.NfrcLogo2 = segments.GetStringAttributeValue("Z58NFRC2");
                logo.NfrcHorizontalOffset = segments.GetDecimalAttributeValue("Z58NFRCH");
                logo.NfrcVerticalOffset = segments.GetDecimalAttributeValue("Z58NFRCV");
                logo.IgLogo = segments.GetStringAttributeValue("Z5840430");
                item.Logo = logo;

            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }
        }

    }
}