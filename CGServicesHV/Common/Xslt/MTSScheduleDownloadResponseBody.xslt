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
          .depth10 {text-indent: 45px;}
          .depth11 {text-indent: 50px;}
          .depth12 {text-indent: 55px;}
          .depth13 {text-indent: 60px;}
          .depth14 {text-indent: 65px;}
          .depth15 {text-indent: 70px;}
          tr.odd { background: #ffffff; }
          tr.even { background: #D5CFCF; }
          tr.stepodd { background: #ffffcc; }
          tr.stepeven { background: #e6ffe6; }
          .defaultTable td
          {
          border: 1px solid black;
          border-collapse:collapse;
          padding:2px;
          }
        </style>
      </head>
      <body>
        <div class="title">HV Schedule</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">Location</td>
            <td align="left">Schedule Num</td>
            <td align="left">E1 Sched Date</td>
            <td align="left">SFS</td>
            <td align="left">ShopFloor System</td>
            <td align="left">E1 Batch Num</td>
            <td align="left">Customers</td>
            <td align="left">Work Orders</td>
            <td align="left">Lites</td>
            <td align="left">Due Date</td>
          </tr>
          <xsl:apply-templates select="SummaryNode">
          </xsl:apply-templates>
        </table>

        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">Run Date</td>
            <td align="left">Elapsed Seconds</td>
            <td align="left">Transfer Status</td>
            <td align="left">Transfer Date</td>
            <td align="left">Force</td>
            <td align="left">Parallel</td>
            <td align="left">Has Errors</td>
            <td align="left">Message</td>
          </tr>
          <xsl:apply-templates select="TransferNode">
          </xsl:apply-templates>
        </table>
        <br/>

        <xsl:if test="//Error">
          <div class="title">Errors</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">Message</td>
              <td align="left">Source</td>
              <td align="left">Stack Trace</td>
            </tr>
            <xsl:apply-templates select="//Error">
            </xsl:apply-templates>
          </table>
          <br/>
        </xsl:if>

        <xsl:if test="ActionLinks/ArrayOfActionLink/ActionLink">
          <div class="title">Schedule Actions</div>
          <ul>
            <xsl:apply-templates select="ActionLinks/ArrayOfActionLink/ActionLink">
            </xsl:apply-templates>
          </ul>
        </xsl:if>

        <xsl:if test="ProcessResponse/Messages/Message">
          <br/>
          <div class="title">Messages</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">Name</td>
              <td align="left">Value</td>
            </tr>
            <xsl:apply-templates select="ProcessResponse/Messages/Message"/>
          </table>
          <br/>
        </xsl:if>

        <xsl:if test="ScheduleCustomerLine/ScheduleCustomerLines/ScheduleCustomerLine">
          <br/>
          <div class="title">Schedule Customer Lines</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">Customer</td>
              <td align="left">Work Order</td>
              <td align="left">Prod Type</td>
              <td align="left">Ansii Logo Type</td>
              <td align="left">Item</td>
              <td align="left">Construction</td>
              <td align="left">Dimensions</td>
              <td align="left">Rack Type</td>
              <td align="right">Lites Qty</td>
            </tr>

            <xsl:apply-templates select="ScheduleCustomerLine/ScheduleCustomerLines/ScheduleCustomerLine">
              <xsl:sort select="CustName"/>
            </xsl:apply-templates>
            <tr>
              <td colspan="8"></td>
              <td align="right" class="numeric">
                <xsl:value-of select="sum(ScheduleCustomerLine/ScheduleCustomerLines/ScheduleCustomerLine/LitesQty)"/>
              </td>
            </tr>
          </table>
        </xsl:if>

        <xsl:if test="ProcessResponse">
          <br/>
          <div class="title">Process Steps</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">Name</td>
              <td align="left">Start Date</td>
              <td align="left">End Date</td>
              <td align="left">Elapsed(Sec)</td>
              <td align="left">Rows Affected</td>
              <td align="left">Messages</td>
            </tr>
            <xsl:apply-templates select="ProcessResponse">
              <xsl:sort select="StartDate"/>
            </xsl:apply-templates>
          </table>
          <br/>
        </xsl:if>

        <xsl:if test="ProcessResponse/ConnectionStrings">
          <div class="title">Connection Strings</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">Name</td>
              <td align="left">Value</td>
            </tr>
            <xsl:apply-templates select="ProcessResponse/ConnectionStrings/ConnectionString">
              <xsl:sort select="Name"/>
            </xsl:apply-templates>
          </table>
          <br/>
        </xsl:if>

        <br/>
        <table class="defaultTable">
          <tr>
            <td>Assembly Version</td>
            <td>
              <xsl:value-of select="//AssemblyVersion"/>
            </td>
          </tr>
          <tr>
            <td>User Name</td>
            <td>
              <xsl:value-of select="//ServiceContext/UserName"/>
            </td>
          </tr>
          <tr>
            <td>Session Id</td>
            <td>
              <xsl:value-of select="//ServiceContext/SessionId"/>
            </td>
          </tr>
          <tr>
            <td>Server Address</td>
            <td>
              <xsl:value-of select="//ServiceContext/ServerAddress"/>
            </td>
          </tr>
        </table>

      </body>
    </html>
  </xsl:template>

  <xsl:template match="SummaryNode/Header">
    <tr>
      <td>
        <xsl:value-of select="Location"/>
      </td>
      <td>
        <xsl:value-of select="ScheduleNum"/>
      </td>
      <td>
        <xsl:value-of select="ScheduleDateE1"/>
      </td>
      <td>
        <xsl:value-of select="ShopFloorSystem"/>
      </td>
      <td>
        <xsl:value-of select="ShopFloorSystemName"/>
      </td>
      <td>
        <xsl:value-of select="BatchNum"/>
      </td>
      <td>
        <xsl:value-of select="Customers"/>
      </td>
      <td>
        <xsl:value-of select="WorkOrders"/>
      </td>
      <td>
        <xsl:value-of select="Lites"/>
      </td>
      <td>
        <xsl:value-of select="ms:format-date(DueDate, 'MM/dd/yyyy','en-US')"/>&#160;
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="TransferNode/Transfer">
    <tr>
      <td>
        <xsl:value-of select="StartDateString"/>
      </td>
      <td>
        <xsl:value-of select="ElapsedSeconds"/>
      </td>
      <td>
        <xsl:value-of select="TransferStatus"/>
      </td>
      <td>
        <xsl:value-of select="ms:format-date(TransferDate, 'MM/dd/yyyy','en-US')"/>&#160;
        <xsl:value-of select="ms:format-time(substring(Transfer/TransferDate, 1, 23 ), 'hh:mm:sstt', 'en-US')"/>
      </td>
      <td>
        <xsl:value-of select="Force"/>
      </td>
      <td>
        <xsl:value-of select="IsParallel"/>
      </td>
      <td>
        <xsl:value-of select="HasErrors"/>
      </td>
      <td>
        <xsl:value-of select="TransferMessage"/>
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

  <xsl:template match="ProcessResponse">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td class="depth{count(ancestor::*)}">
        <xsl:value-of select="ProcessName"/>
      </td>
      <td>
        <xsl:value-of select="StartDate"/>
      </td>
      <td>
        <xsl:value-of select="EndDate"/>
      </td>
      <td align="right">
        <xsl:value-of select="format-number(ElapsedSeconds, '#,##0.00')"/>
      </td>
      <td align="right">
        <xsl:value-of select="RowsAffected"/>
      </td>
      <td>
        <xsl:value-of select="Errors/Error/Message"/>
      </td>
    </tr>
    <xsl:if test="ProcessSteps/ProcessStep">
      <xsl:apply-templates select="ProcessSteps/ProcessStep">
        <xsl:sort select="StartDate"/>
      </xsl:apply-templates>
    </xsl:if>
  </xsl:template>

  <xsl:template match="ProcessStep">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">stepeven</xsl:when>
          <xsl:otherwise>stepeven</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td class="depth{count(ancestor::*)}">
        <xsl:value-of select="Name"/>
      </td>
      <td>
        <xsl:value-of select="StartDate"/>
      </td>
      <td>
        <xsl:value-of select="EndDate"/>
      </td>
      <td align="right">
        <xsl:value-of select="format-number(ElapsedSeconds, '#,##0.00')"/>
      </td>
      <td align="right">
        <xsl:value-of select="RowsAffected"/>
      </td>
      <td>
        <xsl:value-of select="Errors/Error/Message"/>
      </td>
    </tr>
    <xsl:if test="ProcessResponses">
      <xsl:apply-templates select="ProcessResponses/ProcessResponse">
        <xsl:sort select="StartDate"/>
      </xsl:apply-templates>
    </xsl:if>
  </xsl:template>

  <xsl:template match="ConnectionString">
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

  <xsl:template match="//Error">
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

  <xsl:template match="ScheduleCustomerLine/ScheduleCustomerLines/ScheduleCustomerLine">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="DisplayCustomer"/>
      </td>
      <td>
        <xsl:value-of select="WOrderNum"/>
      </td>
      <td>
        <xsl:value-of select="ProdType"/>
      </td>
      <td>
        <xsl:value-of select="AnsiiLogoType"/>
      </td>
      <td>
        <xsl:value-of select="ItemNum"/>
      </td>
      <td>
        <xsl:value-of select="Construction"/>
      </td>
      <td>
        <xsl:value-of select="Dimensions"/>
      </td>
      <td>
        <xsl:value-of select="RackType"/>
      </td>
      <td align="right" class="numeric">
        <xsl:value-of select="LitesQty"/>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>

