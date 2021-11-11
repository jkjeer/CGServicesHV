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
        <div class="title">HV Completion Upload</div>
        <br/>
        <xsl:if test="ArrayOfHvTrimItem">
          <div class="title">HV Completion</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">
                Consumed Glass Id
              </td>
              <td align="left">
                Production Line
              </td>
              <td align="left">
                Serial Number
              </td>
              <td align="right">
                Sheets
              </td>
            </tr>
            <xsl:apply-templates select="ArrayOfHvTrimItem/HvTrimItem">
              <xsl:sort select="GLStockID"/>
            </xsl:apply-templates>
            <tr>
              <td colspan="3"/>
              <td align="right" class="numeric">
                <xsl:value-of select="format-number(sum(ArrayOfHvTrimItem/HvTrimItem/SheetsConsumed), '#,###')"/>
              </td>
            </tr>
          </table>
          <br/>
        </xsl:if>
        <br/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="HvTrimItem">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="GLStockID"/>
      </td>
      <td>
        <xsl:value-of select="ProductionLineCode"/>
      </td>
      <td>
        <xsl:value-of select="RawCrateNumber"/>
      </td>
      <td align="right" >
        <xsl:value-of select="SheetsConsumed"/>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>
