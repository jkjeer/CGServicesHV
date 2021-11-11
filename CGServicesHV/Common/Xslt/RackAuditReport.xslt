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
        <div class="title">Rack Audit Report</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">
              Location
            </td>
            <td align="left">
              E1Only Racks
            </td>
            <td align="left">
              PlantOnly Racks
            </td>
            <td align="left">
              E1 Racks
            </td>
            <td align="left">
              Plant Racks
            </td>
            <td align="right">
              Is Delete
            </td>
            <td align="right">
              Deleted Rows
            </td>
            <td align="right">
              Start Date
            </td>
            <td align="right">
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

        <hr/>

        <div class="title">E1 Only Racks</div>
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
              Status
            </td>
            <td align="left">
              Location
            </td>
            <td align="left">
              Rack Type
            </td>
            <td align="left">
              Transaction Date
            </td>
            <td align="right">
              Quantity
            </td>
          </tr>
          <xsl:choose>
            <xsl:when test="BizOnlyCollection/Racks/Rack">
              <xsl:apply-templates select="BizOnlyCollection/Racks/Rack">
                <xsl:sort select="RackNum"
                          data-type="number"/>
              </xsl:apply-templates>
              <tr class="footerTotals">
                <td colspan="6"/>
                <td align="right"
                    class="numeric">
                  <xsl:value-of select="format-number(sum(BizOnlyCollection/Racks/Rack/Quantity), '#,##0')"/>
                </td>
              </tr>
            </xsl:when>
            <xsl:otherwise>
              <tr>
                <td colspan="7">No Records</td>
              </tr>
            </xsl:otherwise>
          </xsl:choose>
        </table>
        <br/>

        <div class="title">Plant Only Racks</div>
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
              Status
            </td>
            <td align="left">
              Location
            </td>
            <td align="left">
              Rack Type
            </td>
            <td align="left">
              Transaction Date
            </td>
            <td align="right">
              Quantity
            </td>
          </tr>
          <xsl:choose>
            <xsl:when test="PlantOnlyCollection/Racks/Rack">
              <xsl:apply-templates select="PlantOnlyCollection/Racks/Rack">
                <xsl:sort select="RackNum"
                          data-type="number"/>
              </xsl:apply-templates>
              <tr class="footerTotals">
                <td colspan="6"/>
                <td align="right"
                    class="numeric">
                  <xsl:value-of select="format-number(sum(PlantOnlyCollection/Racks/Rack/Quantity), '#,##0')"/>
                </td>
              </tr>
            </xsl:when>
            <xsl:otherwise>
              <tr>
                <td colspan="7">No Records</td>
              </tr>
            </xsl:otherwise>
          </xsl:choose>
        </table>
        <br/>

        <xsl:if test="//ProcessStep[Name='Plant Only Racks']/Messages/Message">
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

        <div class="title">Difference Racks</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td></td>
            <td colspan="4">Business System</td>
            <td colspan="4">Plant System</td>
            <td></td>
          </tr>
          <tr class="head">
            <td align="left">
              Rack Id
            </td>
            <td align="left">
              Status
            </td>
            <td align="left">
              Location
            </td>
            <td align="left">
              Rack Type
            </td>
            <td align="left">
              Transaction Date
            </td>
            <td align="left">
              Status
            </td>
            <td align="left">
              Location
            </td>
            <td align="left">
              Rack Type
            </td>
            <td align="left">
              Transaction Date
            </td>
            <td align="right">
              Quantity
            </td>
          </tr>
          <xsl:choose>
            <xsl:when test="BothDiffCollection/Racks/Rack">
              <xsl:apply-templates select="BothDiffCollection/Racks/Rack">
                <xsl:sort select="RackNum"
                          data-type="number"/>
              </xsl:apply-templates>
              <tr class="footerTotals">
                <td colspan="9"/>
                <td align="right"
                    class="numeric">
                  <xsl:value-of select="format-number(sum(BothDiffCollection/Racks/Rack/Quantity), '#,##0')"/>
                </td>
              </tr>
            </xsl:when>
            <xsl:otherwise>
              <tr>
                <td colspan="10">No Records</td>
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
        <xsl:value-of select="E1OnlyRacks"/>
      </td>
      <td>
        <xsl:value-of select="PlantOnlyRacks"/>
      </td>
      <td>
        <xsl:value-of select="E1Racks"/>
      </td>
      <td>
        <xsl:value-of select="PlantRacks"/>
      </td>
      <td>
        <xsl:value-of select="IsDelete"/>
      </td>
      <td>
        <xsl:value-of select="DeletedRows"/>
      </td>
      <td>
        <xsl:value-of select="StartDate"/>
      </td>
      <td align="right">
        <xsl:value-of select="format-number(ElapsedSeconds, '#,##0.00')"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="BizOnlyCollection/Racks/Rack">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="RackNum"/>
      </td>
      <td>
        <xsl:value-of select="PlantId"/>
      </td>
      <td>
        <xsl:value-of select="StatusCode"/>
      </td>
      <td>
        <xsl:value-of select="Location"/>
      </td>
      <td>
        <xsl:value-of select="RackType"/>
      </td>
      <td>
        <xsl:value-of select="TransactionDate"/>
      </td>
      <td align="right">
        <xsl:value-of select="Quantity"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="PlantOnlyCollection/Racks/Rack">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="RackNum"/>
      </td>
      <td>
        <xsl:value-of select="PlantId"/>
      </td>
      <td>
        <xsl:value-of select="StatusCode"/>
      </td>
      <td>
        <xsl:value-of select="Location"/>
      </td>
      <td>
        <xsl:value-of select="RackType"/>
      </td>
      <td>
        <xsl:value-of select="TransactionDate"/>
      </td>
      <td align="right">
        <xsl:value-of select="Quantity"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="BothDiffCollection/Racks/Rack">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="RackNum"/>
      </td>
      <td>
        <xsl:value-of select="BizStatusCode"/>
      </td>
      <td>
        <xsl:value-of select="BizLocation"/>
      </td>
      <td>
        <xsl:value-of select="BizRackType"/>
      </td>
      <td>
        <xsl:value-of select="BizTransactionDate"/>
      </td>
      <td>
        <xsl:value-of select="PlantStatusCode"/>
      </td>
      <td>
        <xsl:value-of select="PlantLocation"/>
      </td>
      <td>
        <xsl:value-of select="PlantRackType"/>
      </td>
      <td>
        <xsl:value-of select="PlantTransactionDate"/>
      </td>
      <td align="right">
        <xsl:value-of select="Quantity"/>
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

</xsl:stylesheet>
