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
        <div class="title">Cust Pack Upload</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">
              Plant Num
            </td>
            <td align="left">
              Plant Code
            </td>
            <td align="left">
              Batch Num
            </td>
            <td align="left">
              Start Date
            </td>
            <td align="left">
              End Date
            </td>
            <td align="right">
              ElapsedSeconds
            </td>
          </tr>
          <xsl:apply-templates select="Header/Header">
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
               
        <xsl:if test="ValidationErrors">
          <div class="title">Validation Items</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">
                Cust Pack Id
              </td>
              <td align="left">
                Schedule
              </td>
              <td align="left">
                Work Order
              </td>
              <td align="left">
                Serial Num
              </td>
              <td align="left">
                Rack Type
              </td>
              <td align="left">
                Box/Slot
              </td>
              <td align="left">
                Location
              </td>
              <td align="left">
                Transfer
              </td>
              <td align="left">
                Transfer Id
              </td>
              <td align="left">
                Message
              </td>
              <td align="left">
                Backout
              </td>
              <td align="right">
                Quantity
              </td>
            </tr>
            <xsl:apply-templates select="ValidationErrors/ArrayOfCustPackItem">
              <xsl:sort select="CustPackId"/>
            </xsl:apply-templates>
            <tr>
              <td colspan="11"/>
              <td align="right" class="numeric">
                <xsl:value-of select="format-number(sum(ValidationErrors/ArrayOfCustPackItem/CustPackItem/Quantity), '#,###')"/>
              </td>
            </tr>
          </table>
          <br/>
        </xsl:if>

        <xsl:if test="ProdCustPacks">
          <div class="title">Cust Pack Items</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">
                Cust Pack Id
              </td>
              <td align="left">
                Schedule
              </td>
              <td align="left">
                Work Order
              </td>
              <td align="left">
                Serial Num
              </td>
              <td align="left">
                Rack Type
              </td>
              <td align="left">
                Box/Slot
              </td>
              <td align="left">
                Location
              </td>
              <td align="left">
                Transfer
              </td>
              <td align="left">
                Transfer Id
              </td>
              <td align="left">
                Message
              </td>
              <td align="left">
                Backout
              </td>              
              <td align="right">
                Quantity
              </td>
            </tr>
            <xsl:apply-templates select="ProdCustPacks/ArrayOfCustPackItem">
              <xsl:sort select="CustPackId"/>
            </xsl:apply-templates>
            <tr>
              <td colspan="11"/>
              <td align="right" class="numeric">
                <xsl:value-of select="format-number(sum(ProdCustPacks/ArrayOfCustPackItem/CustPackItem/Quantity), '#,###')"/>
              </td>
            </tr>
          </table>
          <br/>
        </xsl:if>

        <xsl:if test="ProcessResponse">
          <div class="title">Process Steps</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
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
                Messages
              </td>
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

        <br/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="Header">
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
        <xsl:value-of select="PlantCode"/>
      </td>
      <td>
        <xsl:value-of select="BatchNum"/>
      </td>
      <td>
        <xsl:value-of select="StartDate"/>
      </td>
      <td>
        <xsl:value-of select="EndDate"/>
      </td>
      <td>
        <xsl:value-of select="ElapsedSeconds"/>
      </td>
    </tr>
  </xsl:template>
  
  <xsl:template match="ProdCustPacks/ArrayOfCustPackItem/CustPackItem">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="CustPackId"/>
      </td>
      <td>
        <xsl:value-of select="ScheduleNum"/>
      </td>
      <td>
        <xsl:value-of select="WorkOrderNum"/>
      </td>
      <td>
        <xsl:value-of select="SerialNum"/>
      </td>
      <td>
        <xsl:value-of select="RackType"/>
      </td>
      <td>
        <xsl:value-of select="BoxSlot"/>
      </td>
      <td>
        <xsl:value-of select="Location"/>
      </td>
      <td>
        <xsl:value-of select="TransferStatus"/>
      </td>
      <td>
        <xsl:value-of select="TransferId"/>
      </td>
      <td>
        <xsl:value-of select="TransferComment"/>
      </td>
      <td>
        <xsl:value-of select="Backout"/>
      </td>      
      <td align="right" >
        <xsl:value-of select="Quantity"/>
      </td>
    </tr>
  </xsl:template>


  <xsl:template match="ValidationErrors/ArrayOfCustPackItem/CustPackItem">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="CustPackId"/>
      </td>
      <td>
        <xsl:value-of select="ScheduleNum"/>
      </td>
      <td>
        <xsl:value-of select="WorkOrderNum"/>
      </td>
      <td>
        <xsl:value-of select="SerialNum"/>
      </td>
      <td>
        <xsl:value-of select="RackType"/>
      </td>
      <td>
        <xsl:value-of select="BoxSlot"/>
      </td>
      <td>
        <xsl:value-of select="Location"/>
      </td>
      <td>
        <xsl:value-of select="TransferStatus"/>
      </td>
      <td>
        <xsl:value-of select="TransferId"/>
      </td>
      <td>
        <xsl:value-of select="TransferComment"/>
      </td>
      <td>
        <xsl:value-of select="Backout"/>
      </td>
      <td align="right" >
        <xsl:value-of select="Quantity"/>
      </td>
    </tr>
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
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
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

  
</xsl:stylesheet>
