<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="html"/>

    <xsl:template match="Config">
      <html>
        <head>
          <style>
            ul { list-style-type: none;}
            li { margin: 0 0 3px 0;}
            label {display: inline-block; width:140px; text-align: left;}
            .configInfo input {width: 250px;}
            .column {float:left;}
            .columns ul {border-style: inset inset outset outset; margin: 3px;}
            .addcolumn {clear: left;}
          </style>
        </head>
        <body>
          <p>
            <div class="configInfo">
              <ul>
                <xsl:for-each select="@*">
                  <li>
                    <label>
                      <xsl:value-of select="translate(name(),'_',' ')"/>
                    </label>
                    <input type="text">
                      <xsl:attribute name="value">
                        <xsl:value-of select="." />
                      </xsl:attribute>
                    </input>
                  </li>
                </xsl:for-each>          
              </ul>
            </div>
          </p>
          <p>
            <div class="columns">              
                <xsl:for-each select="Columns/Column">
                  <div class="column">
                    <ul>
                      <xsl:for-each select="@*">
                        <li>
                          <label>
                            <xsl:value-of select="translate(name(),'_',' ')"/>
                          </label>
                          <input type="text">
                            <xsl:attribute name="value">
                              <xsl:value-of select="." />
                            </xsl:attribute>
                          </input>
                        </li>
                      </xsl:for-each>
                    </ul>
                  </div>
                </xsl:for-each>
              <div class="addcolumn">
                <ul>
                    <li>
                      <label>
                        <xsl:value-of select="translate(name(),'_',' ')"/>
                      </label>
                      <input type="text">
                      </input>
                    </li>
                  <li>
                    <input type="button" value="Add Column"/>
                  </li>
                </ul>
              </div>
            </div>
          </p>
        </body>
      </html>
    </xsl:template>
</xsl:stylesheet>
