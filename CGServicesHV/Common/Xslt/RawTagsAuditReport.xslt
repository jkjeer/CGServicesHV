<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:ms="urn:schemas-microsoft-com:xslt">

	<xsl:template match="root">
		<html>
			<head>
				<style>
					body,td,p,th  { font-family: arial; font-size: 8pt; }
					tr.head { border : 0px solid black; padding: 0 0px; background: #ddddff; font-weight:bold;}
					td.head { border : 0px solid black; padding: 0 0px; background: #ddddff; }
					td.numeric { text-align:right; width:80px;  }
					td.noBorder {border: none; }
					.error {color: red;}
					.title {font-weight:bold; text-decoration: underline;}
					.depth1 {text-indent: 0px;}
					.depth2 {text-indent: 5px;}
					.depth3 {text-indent: 10px;}
					.depth4 {text-indent: 15px;}
					.depth5 {text-indent: 20px;}
					.depth6 {text-indent: 25px;}
					.depth7 {text-indent: 30px;}
					.depth8 {text-indent: 35px;}
					.depth9 {text-indent: 40px;}
					.footerTotals {font-weight:bold;color: white;background: #565656; }
					tr.odd { background: #ffffff; }
					tr.even { background: #D5CFCF; }
					.defaultTable td, table
					{
					border: 1px solid black;
					border-collapse:collapse;
					padding:2px;
					}
				</style>
			</head>
			<body>
				<div class="title">Raw Tags Audit Report</div>
				<br/>
				<table class="defaultTable">
					<tr class="head">
						<td align="left">
							Location
						</td>
						<td align="left">
							E1Only RawTags
						</td>
						<td align="left">
							PlantOnly RawTags
						</td>
						<td align="left">
							E1 RawTags
						</td>
						<td align="left">
							Plant RawTags
						</td>
						<td align="left">
							Start Date
						</td>
						<td align="left">
							Elapsed Seconds
						</td>
					</tr>
					<xsl:apply-templates select="ReportHeader/ReportHeader">
					</xsl:apply-templates>
				</table>
				<br/>

				<xsl:if test="//Error">
					<div class="title">Errors</div>
					<br/>
					<table class="defaultTable">
						<tr class="head">
							<td align="left">
								Message
							</td>
							<td align="left">
								Source
							</td>
							<td align="left">
								Stack Trace
							</td>
						</tr>
						<xsl:apply-templates select="//Error">
						</xsl:apply-templates>
					</table>
					<br/>
				</xsl:if>

				<xsl:if test="ActionLinks/ArrayOfActionLink/ActionLink">
					<br/>
					<div class="title">Actions</div>
					<ul>
						<xsl:apply-templates select="ActionLinks/ArrayOfActionLink/ActionLink">
						</xsl:apply-templates>
					</ul>
				</xsl:if>

				<hr/>

				<div class="title">E1 Only RawTags</div>
				<br/>
				<table class="defaultTable">
					<tr class="head">
						<td align="left">
							Rack Id
						</td>
						<td align="left">
							Plant Id
						</td>
						<td align="left">
							Serial Num
						</td>
						<td align="left">
							Lot Num
						</td>
						<td align="left">
							Item Num
						</td>
						<td align="left">
							Status
						</td>
						<td align="right">
							Quantity
						</td>
						<td align="left">
							Location
						</td>
						<td align="left">
							Transaction Date
						</td>
					</tr>
					<xsl:choose>
						<xsl:when test="BizOnlyCollection/RawTags/RawTag">
							<xsl:apply-templates select="BizOnlyCollection/RawTags/RawTag">
								<xsl:sort select="LotNum"
										  data-type="text"/>
								<xsl:sort select="SerialNum"
										  data-type="text"/>
							</xsl:apply-templates>
							<tr class="footerTotals">
								<td colspan="6"/>
								<td align="right"
									class="numeric">
									<xsl:value-of select="format-number(sum(BizOnlyCollection/RawTags/RawTag/Quantity), '#,##0')"/>
								</td>
							</tr>
						</xsl:when>
						<xsl:otherwise>
							<tr>
								<td colspan="9">No Records</td>
							</tr>
						</xsl:otherwise>
					</xsl:choose>
				</table>
				<br/>

				<div class="title">Plant Only RawTags</div>
				<br/>
				<table class="defaultTable">
					<tr class="head">
						<td align="left">
							Rack Id
						</td>
						<td align="left">
							Plant Id
						</td>
						<td align="left">
							Serial Num
						</td>
						<td align="left">
							Lot Num
						</td>
						<td align="left">
							Item Num
						</td>
						<td align="left">
							Status
						</td>
						<td align="right">
							Quantity
						</td>
						<td align="left">
							Location
						</td>
						<td align="left">
							Transaction Date
						</td>
					</tr>
					<xsl:choose>
						<xsl:when test="PlantOnlyCollection/RawTags/RawTag">
							<xsl:apply-templates select="PlantOnlyCollection/RawTags/RawTag">
								<xsl:sort select="LotNum"
										  data-type="text"/>
								<xsl:sort select="SerialNum"
										  data-type="text"/>
							</xsl:apply-templates>
							<tr class="footerTotals">
								<td colspan="6"/>
								<td align="right"
									class="numeric">
									<xsl:value-of select="format-number(count(PlantOnlyCollection/RawTags/RawTag/Quantity), '#,##0')"/>
								</td>
							</tr>
						</xsl:when>
						<xsl:otherwise>
							<tr>
								<!--<td colspan="8">No Records</td>-->
								<td colspan="9">
									<xsl:value-of select="//ReportHeader/PlantOnlyRawTagsLabel"/>
								</td>
							</tr>
						</xsl:otherwise>
					</xsl:choose>
				</table>
				<br/>

				<xsl:if test="//ProcessStep[Name='Plant Only RawTags']/Messages/Message">
					<div class="title">Messages</div>
					<br/>
					<table class="defaultTable">
						<tr class="head">
							<td align="left">Name</td>
							<td align="left">Value</td>
						</tr>
						<xsl:apply-templates select="//Message">
							<xsl:sort select="Name"/>
						</xsl:apply-templates>
					</table>
					<br/>
				</xsl:if>

				<div class="title">Difference RawTags</div>
				<br/>
				<table class="defaultTable">
					<tr class="head">
						<td colspan="2"></td>
						<td colspan="6">Business System</td>
						<td colspan="6">Plant System</td>
					</tr>
					<tr class="head">
						<td align="left">
							Serial Num
						</td>
						<td align="left">
							Lot Num
						</td>
						<td align="left">
							Item Num
						</td>
						<td align="left">
							Rack Id
						</td>
						<td align="left">
							Status
						</td>
						<td align="right">
							Quantity
						</td>
						<td align="left">
							Location
						</td>
						<td align="left">
							TransactionDate
						</td>
						<td align="left">
							Item Num
						</td>
						<td align="left">
							Rack Id
						</td>
						<td align="left">
							Status
						</td>
						<td align="right">
							Quantity
						</td>
						<td align="left">
							Location
						</td>
						<td align="left">
							TransactionDate
						</td>
					</tr>
					<xsl:choose>
						<xsl:when test="BothDiffCollection/RawTags/RawTag">
							<xsl:apply-templates select="BothDiffCollection/RawTags/RawTag">
								<xsl:sort select="SerialNum"
										  data-type="text"/>
								<xsl:sort select="LotNum"
										  data-type="text"/>
							</xsl:apply-templates>
							<tr class="footerTotals">
								<td colspan="5"/>
								<td align="right"
									class="numeric">
									<xsl:value-of select="format-number(sum(BothDiffCollection/RawTags/RawTag/BizQuantity), '#,##0')"/>
								</td>
								<td colspan="5"/>
								<td align="right"
									class="numeric">
									<xsl:value-of select="format-number(sum(BothDiffCollection/RawTags/RawTag/PlantQuantity), '#,##0')"/>
								</td>
							</tr>
						</xsl:when>
						<xsl:otherwise>
							<tr>
								<td colspan="14">No Records</td>
							</tr>
						</xsl:otherwise>
					</xsl:choose>
				</table>
				<br/>

				<hr/>

				<xsl:if test="ProcessResponse/ProcessSteps">
					<div class="title">Process Steps</div>
					<br/>
					<table class="defaultTable">
						<tr class="head">
							<td align="left">
								Id
							</td>
							<td align="left">
								Name
							</td>
							<td align="left">
								Start Date
							</td>
							<td align="left">
								End Date
							</td>
							<td align="left">
								Elapsed(Sec)
							</td>
							<td align="left">
								Rows Affected
							</td>
							<td align="left">
								Has Errors
							</td>
							<td align="left">
								Messages
							</td>
						</tr>
						<xsl:apply-templates select="ProcessResponse/ProcessSteps/ProcessStep">
							<xsl:sort select="StartDate"/>
						</xsl:apply-templates>
						<tr>
							<td colspan="4"/>
							<td align="right"
								class="numeric">
								<xsl:value-of select="format-number(sum(ProcessResponse/ProcessSteps/ProcessStep/ElapsedSeconds), '#,##0.00')"/>
							</td>
							<td colspan="2"/>
						</tr>
					</table>
					<br/>
				</xsl:if>

				<xsl:if test="ProcessResponse/ConnectionStrings">
					<div class="title">Connection Strings</div>
					<br/>
					<table class="defaultTable">
						<tr class="head">
							<td align="left">
								Name
							</td>
							<td align="left">
								Value
							</td>
						</tr>
						<xsl:apply-templates select="ProcessResponse/ConnectionStrings/ConnectionString">
							<xsl:sort select="Name"/>
						</xsl:apply-templates>
					</table>
					<br/>
				</xsl:if>

				<xsl:if test="ServiceContext">
					<div class="title">Service Context</div>
					<br/>
					<table class="defaultTable">
						<xsl:apply-templates select="ServiceContext/ServiceContext">
						</xsl:apply-templates>
					</table>
					<br/>
				</xsl:if>

				<br/>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="ReportHeader">
		<tr>
			<xsl:attribute name="class">
				<xsl:choose>
					<xsl:when test="position() mod 2 = 0">even</xsl:when>
					<xsl:otherwise>odd</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<td>
				<xsl:value-of select="Plant"/>
			</td>
			<td>
				<xsl:value-of select="E1OnlyRawTags"/>
			</td>
			<td>
				<xsl:value-of select="PlantOnlyRawTags"/>
			</td>
			<td>
				<xsl:value-of select="E1RawTags"/>
			</td>
			<td>
				<xsl:value-of select="PlantRawTags"/>
			</td>
			<td>
				<xsl:value-of select="StartDate"/>
			</td>
			<td align="right">
				<xsl:value-of select="format-number(ElapsedSeconds, '#,##0.00')"/>
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="BizOnlyCollection/RawTags/RawTag">
		<tr>
			<xsl:attribute name="class">
				<xsl:choose>
					<xsl:when test="position() mod 2 = 0">even</xsl:when>
					<xsl:otherwise>odd</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<td>
				<xsl:value-of select="RackId"/>
			</td>
			<td>
				<xsl:value-of select="PlantId"/>
			</td>
			<td>
				<xsl:value-of select="SerialNum"/>
			</td>
			<td>
				<xsl:value-of select="LotNum"/>
			</td>
			<td>
				<xsl:value-of select="ItemNum"/>
			</td>
			<td>
				<xsl:value-of select="Status"/>
			</td>
			<td align="right">
				<xsl:value-of select="Quantity"/>
			</td>
			<td>
				<xsl:value-of select="Location"/>
			</td>
			<td>
				<xsl:value-of select="TransactionDate"/>
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="PlantOnlyCollection/RawTags/RawTag">
		<tr>
			<xsl:attribute name="class">
				<xsl:choose>
					<xsl:when test="position() mod 2 = 0">even</xsl:when>
					<xsl:otherwise>odd</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<td>
				<xsl:value-of select="RackId"/>
			</td>
			<td>
				<xsl:value-of select="PlantId"/>
			</td>
			<td>
				<xsl:value-of select="SerialNum"/>
			</td>
			<td>
				<xsl:value-of select="LotNum"/>
			</td>
			<td>
				<xsl:value-of select="ItemNum"/>
			</td>
			<td>
				<xsl:value-of select="Status"/>
			</td>
			<td align="right">
				<xsl:value-of select="Quantity"/>
			</td>
			<td>
				<xsl:value-of select="Location"/>
			</td>
			<td>
				<xsl:value-of select="TransactionDate"/>
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="BothDiffCollection/RawTags/RawTag">
		<tr>
			<xsl:attribute name="class">
				<xsl:choose>
					<xsl:when test="position() mod 2 = 0">even</xsl:when>
					<xsl:otherwise>odd</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<td>
				<xsl:value-of select="SerialNum"/>
			</td>
			<td>
				<xsl:value-of select="LotNum"/>
			</td>
			<td>
				<xsl:value-of select="BizItemNum"/>
			</td>
			<td>
				<xsl:value-of select="BizRackId"/>
			</td>
			<td>
				<xsl:value-of select="BizStatus"/>
			</td>
			<td align="right">
				<xsl:value-of select="BizQuantity"/>
			</td>
			<td>
				<xsl:value-of select="BizLocation"/>
			</td>
			<td>
				<xsl:value-of select="BizTransactionDate"/>
			</td>
			<td>
				<xsl:value-of select="PlantItemNum"/>
			</td>
			<td>
				<xsl:value-of select="PlantRackId"/>
			</td>
			<td>
				<xsl:value-of select="PlantStatus"/>
			</td>
			<td align="right">
				<xsl:value-of select="PlantQuantity"/>
			</td>
			<td>
				<xsl:value-of select="PlantLocation"/>
			</td>
			<td>
				<xsl:value-of select="PlantTransactionDate"/>
			</td>
		</tr>
	</xsl:template>


	<xsl:template match="ProcessResponse/ProcessSteps/ProcessStep">
		<tr>
			<xsl:attribute name="class">
				<xsl:choose>
					<xsl:when test="position() mod 2 = 0">even</xsl:when>
					<xsl:otherwise>odd</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<td>
				<xsl:value-of select="Id"/>
			</td>
			<td>
				<xsl:value-of select="Name"/>
			</td>
			<td>
				<xsl:value-of select="StartDateChar"/>
			</td>
			<td>
				<xsl:value-of select="EndDateChar"/>
			</td>
			<td align="right">
				<xsl:value-of select="format-number(ElapsedSeconds, '#,##0.00')"/>
			</td>
			<td align="right">
				<xsl:value-of select="RowsAffected"/>
			</td>
			<td>
				<xsl:value-of select="HasErrors"/>
			</td>
			<td>
				<xsl:value-of select="Errors/Error/Message"/>
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="ProcessResponse/ConnectionStrings/ConnectionString">
		<tr>
			<xsl:attribute name="class">
				<xsl:choose>
					<xsl:when test="position() mod 2 = 0">even</xsl:when>
					<xsl:otherwise>odd</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<td>
				<xsl:value-of select="Name"/>
			</td>
			<td>
				<xsl:value-of select="Value"/>
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="ServiceContext/ServiceContext">
		<tr>
			<td>Host Name</td>
			<td>
				<xsl:value-of select="HostName"/>
			</td>
		</tr>
		<tr>
			<td>User Name</td>
			<td>
				<xsl:value-of select="UserName"/>
			</td>
		</tr>
		<tr>
			<td>Scheme</td>
			<td>
				<xsl:value-of select="Scheme"/>
			</td>
		</tr>
		<tr>
			<td>Server Address</td>
			<td>
				<xsl:value-of select="ServerAddress"/>
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="Error">
		<tr>
			<xsl:attribute name="class">
				<xsl:choose>
					<xsl:when test="position() mod 2 = 0">even</xsl:when>
					<xsl:otherwise>odd</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<td>
				<xsl:value-of select="Message"/>
			</td>
			<td>
				<xsl:value-of select="Source"/>
			</td>
			<td>
				<xsl:value-of select="StackTrace"/>
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="Message">
		<tr>
			<xsl:attribute name="class">
				<xsl:choose>
					<xsl:when test="position() mod 2 = 0">even</xsl:when>
					<xsl:otherwise>odd</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<td>
				<xsl:value-of select="Name"/>
			</td>
			<td>
				<xsl:value-of select="Value"/>
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="ActionLink">
		<li>
			<xsl:element name="a">
				<xsl:attribute name="href">
					<xsl:value-of select="Href"/>
				</xsl:attribute>
				<xsl:value-of select="Description"/>
			</xsl:element>
		</li>
	</xsl:template>

</xsl:stylesheet>
