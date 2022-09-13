var shrinkFaktor = 0.88
var textAlign = "text-align";
var alignGridCaption = "center"
function showHidePassword(id) {
    var tcNo = document.getElementById(id);
    var tcType = tcNo.getAttribute("type");
    //alert(tcType);
    if (tcType == "password") {
        tcNo.setAttribute("type", "text");
    }
    else {
        tcNo.setAttribute("type", "password");
    }
}

function gridCompleteOrtak(gridID, headerCursor) 
{
    alignGridHeaders(jQuery(gridID));
    setGridCaptionStyle(jQuery(gridID), textAlign, alignGridCaption);
    //alert(headerCursor);
    if (headerCursor)
    {
        //alert("girdiHeaderCursor");
        jqGridHeaderCursor(gridID);
    }
        //applyClassesToHeaders(jQuery(gridID));
}

function alignGridHeaders(gridObj) {
    var cm = gridObj.jqGrid("getGridParam", "colModel");
    for (var i = 0; i < cm.length; i++) {
        gridObj.jqGrid('setLabel', cm[i].name, '',
            { 'text-align': (cm[i].align || 'left') },
            (cm[i].titletext ? { 'title': cm[i].titletext } : {}));
    }
}

function setGridCaptionStyle(gridObj, cssName, cssValue) {

    gridObj.closest("div.ui-jqgrid-view")
        .children("div.ui-jqgrid-titlebar")
        .css(cssName, cssValue)
        .children("span.ui-jqgrid-title")
        .css("float", "none");
}

//Takes css classes assigned to each column in the jqGrid colModel 
//and applies them to the associated header.
var applyClassesToHeaders = function (grid) {
    // Use the passed in grid as context, 
    // in case we have more than one table on the page.
    var trHead = jQuery("thead:first tr", grid.hdiv);
    var colModel = grid.getGridParam("colModel");

    for (var iCol = 0; iCol < colModel.length; iCol++) {
        var columnInfo = colModel[iCol];
        if (columnInfo.class) {
            var headDiv = jQuery("th:eq(" + iCol + ") div", trHead);
            headDiv.addClass(columnInfo.class);
        }
    }
};

function webBase()
{
    //alert(document.location.pathname);
   var paths = document.location.pathname.split("/");
   return document.location.origin + paths[1];
}

function appletIptal(summary) {

    $("#appletDiv").hide();

    var SS = document.getElementById("appletDiv");
    SS.innerHTML = "";

    $("#divIslem").show();

    $('#divAna').find('input, textarea, button, select').attr('disabled', false);

}

function createDate(yil, ay, gun)
{
    var tarih = new Date(yil, ay, gun);
    alert(tarih);
}
function changeDMYtoDate(DMY)
{
    DMY = DMY.replace(".", "-");
    DMY = DMY.replace("/", "-");
    var parts = DMY.split('-');
    var mydate = new Date(parts[2], parts[1] - 1, parts[0]);
    return mydate;
}

var _MS_PER_DAY = 1000 * 60 * 60 * 24;

// a and b are javascript Date objects
function dateDiffInDays(a, b)
{
    // Discard the time and time-zone information.
    var utc1 = Date.UTC(a.getFullYear(), a.getMonth(), a.getDate());
    var utc2 = Date.UTC(b.getFullYear(), b.getMonth(), b.getDate());

    return Math.floor((utc2 - utc1) / _MS_PER_DAY);
}

function computeDaysWithoutHolidays(bas, bit)
{
    //alert("computeDaysWithoutHolidays başla");
    var tar = new Date(bas);
    //alert("tar=>"); alert(tar);
    //alert("bit=>"); alert(bit);
    var say = 0;
    while (tar <= bit)
    {
        //alert(tar);
        var dayOfWeek = tar.getDay();
        if (dayOfWeek != 0 && dayOfWeek != 6) {
            say++
        }

        tar.setDate(tar.getDate() + 1);
    }
       
    //alert(bas);
    //alert(tar);
    return say;
}

function timeDiffInMinutes(time1,time2)
{
    //alert("timeDiffInMinutes basla="+ time1 + "===" + time2);
    var parts1 = time1.split(':');
    var parts2 = time2.split(':');
    var minutes = (parseInt(parts2[0]) - parseInt(parts1[0])) * 60 + parseInt(parts2[1]) - parseInt(parts1[1])

    //alert("sureDakika=>" + minutes);
    return minutes;
}

function GunSaatDakika(dakikaToplam)
{
    var gun = Math.floor(dakikaToplam / 480);
    var dakikalar = dakikaToplam % 480
    var saat = Math.floor(dakikalar / 60);
    var dakika = dakikalar % 60

    var gunKismi = gun == 0 ? "" : gun + " gün";
    var saatKismi = saat == 0 ? "" : " " + saat + " saat"
    var dakikaKismi = dakika == 0 ? "" : " " + dakika + " dakika"
    return gunKismi + saatKismi + dakikaKismi;
}

function SureHesapla(basTar, bitTar, basSaat, bitSaat) {
    //alert("SureHesapla Parametreli Başla");
    //var gunler = dateDiffInDays(basTar, bitTar);
    //alert(basTar);
    //alert(bitTar);
    var gunler = computeDaysWithoutHolidays(basTar, bitTar);
    //alert("gunler=>" + gunler);
    var ilkGunSay = computeDaysWithoutHolidays(basTar, basTar);
    var sonGunSay = computeDaysWithoutHolidays(bitTar, bitTar);

    //alert("ilk timeDiffInMinutes çağırma");
    if (basTar.getTime() == bitTar.getTime()) {
        //alert("bastarTime=bitTarTime");
        return gunler * (timeDiffInMinutes(basSaat, bitSaat) - haricHesapla(basSaat, bitSaat));
    }

    //alert(gunler + "-" + ilkGunSay + "-" + sonGunSay);
    var ilkGunSure = ilkGunSay * (timeDiffInMinutes(basSaat, "18:00") - haricHesapla(basSaat, "18:00"));
    //alert("ilkGunSure=>"+ilkGunSure);
    var gunlerSure = 0;
    var sonGunSure = 0;
    if (gunler - ilkGunSay - sonGunSay > 0)
    {
        gunlerSure = 480 * (gunler - ilkGunSay - sonGunSay);
    }
    //alert("günler:" + gunlerSure);
    var sureSaat = timeDiffInMinutes("09:00", bitSaat);
    
    sonGunSure = sonGunSay * (timeDiffInMinutes("09:00", bitSaat) - haricHesapla("09:00", bitSaat));
    //alert("sonGunSure:" + sonGunSure);
    var toplamSure = ilkGunSure + gunlerSure + sonGunSure;

    return toplamSure;

    //return (gunler) * (dakika - haric);
}

function SureHesaplaGorev(basSaat, bitSaat) {

    return (timeDiffInMinutes(basSaat, bitSaat) - haricHesapla(basSaat, bitSaat));

}

function haricHesapla(basSaat, bitSaat)
{
    //alert("haricHesapla");
    var haric = 0;
    if (basSaat <= '12:30' && bitSaat >= '12:30' && bitSaat <= '13:30')
        haric = timeDiffInMinutes('12:30', bitSaat);
    if (basSaat <= '12:30' && bitSaat >= '13:30')
        haric = 60;
    if (basSaat <= '12:30' && bitSaat >= '18:00')
        haric = 60 + timeDiffInMinutes('18:00', bitSaat);
    if (basSaat >= '12:30' && basSaat <= '13:30' && bitSaat <= '13:30')
        haric = timeDiffInMinutes(basSaat, bitSaat);
    if (basSaat >= '12:30' && basSaat <= '13:30' && bitSaat >= '13:30')
        haric = timeDiffInMinutes(basSaat, '13:30');
    if (basSaat >= '12:30' && basSaat <= '13:30' && bitSaat >= '18:00')
        haric = timeDiffInMinutes(basSaat, '13:30') + timeDiffInMinutes('18:00', bitSaat);
    if (basSaat >= '13:30' && bitSaat >= '18:00')
        haric = timeDiffInMinutes('18:00', bitSaat);
    if (basSaat >= '18:00' && bitSaat >= '18:00')
        haric = timeDiffInMinutes(basSaat, bitSaat);

    //alert("haric:" + haric);
    return haric;

}

var createExcelFromGrid = function (gridID, filename) {
    var grid = $('#' + gridID);

    var html = "";
    //var headers = grid[0].grid.headers;
    //var headers = grid[0].p.colNames;
    //for (var header in headers)
    //{
    //    html = html + header +'\t'; // Capture Column Names
    //}

    var rowIDList = grid.jqGrid(getDataIDs());
    var row = grid.getRowData(rowIDList[0]);
    var colNames = [];
    var i = 0;
    for (var cName in row) {
        colNames[i++] = cName; // Capture Column Names
    }
    for (var j = 0; j < rowIDList.length; j++) {
        row = grid.getRowData(rowIDList[j]); // Get Each Row
        for (var i = 0 ; i < colNames.length ; i++) {
            html += row[colNames[i]] + '\t'; // Create a CSV delimited with ;
        }
        html += '\n';
    }
    html += '\n';

    alert(html);

    //alert("yeni");
    var a = document.createElement('a');
    a.id = 'ExcelDL';
    //a.href = 'data:application/vnd.ms-excel,' + html;
    a.href = 'data:xls;charset=utf-8,' + escape(html);

    a.download = filename ? filename + ".xls" : 'DataList.xls';
    document.body.appendChild(a);
    a.click(); // Downloads the excel document
    document.getElementById('ExcelDL').remove();
}


function ExportToExcel(grid, ReportTitle, label)
{
    alert("****************GİRDİ*********************")
    var cm = grid.jqGrid("getGridParam", "colModel");

    var caption = grid.jqGrid("getGridParam", "caption");
    var headerRow = grid.jqGrid("getGridParam", "colNames");

    var dataSource = grid.jqGrid('getRowData');

    var arrData;
        arrData = typeof dataSource != 'object' ? JSON.parse(dataSource) : dataSource;
        //alert(arrData.length);
            var Excel = '';    
            //Set Report title in first row or line
            //Excel += ReportTitle + '\r\n\n';
    
            //This condition will generate the Label/Header
            var baslik = "";
            if (label)
            {
                baslik = baslikHazirla(grid, arrData);
                //alert(baslik);
                Excel += baslik + '\r\n';
            }
        
            //1st loop is to extract each row
            for (var i = 0; i < arrData.length; i++) {
                    var row = "";
                
                    //2nd loop will extract each column and convert it in string comma-seprated
                    for (var index in arrData[i]) {
                            row += '"' + arrData[i][index] + '";';
                        }
        
                    row.slice(0, row.length - 1);
                   
                    Excel += row + '\r\n';
                }
    
            if (Excel == '') {        
                    return;
                }   
        
            //Generate a file name
            var fileName = "";
            //this will remove the blank-spaces from the title and replace it with an underscore
            fileName += ReportTitle.replace(/ /g,"_");   
        
            var universalBOM = "\uFEFF";
            //var uri = 'data:xls;charset=windows-1252,' + escape(Excel);
            var uri = 'data:xls;charset=utf-8,' + encodeURIComponent(universalBOM + Excel);
            var link = document.createElement("a");    
            link.href = uri;
        
            alert(Excel);
            link.style = "visibility:hidden";
            link.download = fileName + ".xls";
        
            //this part will append the anchor tag and remove it after automatic click
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
}

function baslikHazirla(grid, data)
{
   
    var caption = grid.jqGrid("getGridParam", "caption");
    var headerRow = grid.jqGrid("getGridParam", "colNames");

    var kolonSay = headerRow.length;
    var ortaOncesi = kolonSay / 2;
    var ortaSonrasi = kolonSay - 1 - ortaOncesi;

    var baslik = "";
    for (var ix = 1; ix <= ortaOncesi; ix++) {
        baslik = baslik + '' + ';';
    }
    baslik = baslik + caption + ';';
    for (var ix = 1; ix <= ortaSonrasi; ix++) {
        baslik = baslik + '' + ';';
    }
   baslik += '\r\n';

   //alert(baslik);

   var grupBaslik = grupBaslikHazirla(grid, data);
   //alert(grupBaslik);
   baslik += grupBaslik + '\r\n';
   var row = "";
   for (var i = 0; i < headerRow.length; i++) {
        row += headerRow[i] + ';';
    }
    row = row.slice(0, -1);


    //append Label row with line break
    baslik += row + '\r\n';

    return baslik;
}

function grupBaslikHazirla(grid, data)
{
    var groupHeader = grid.jqGrid("getGridParam", "groupHeader");
    if (groupHeader == null)
        return "";
    var grupSay = groupHeader.groupHeaders.length;
    var gruplar = groupHeader.groupHeaders;
    //alert(grupSay);
    var ilkKolon = -1;
    var grupBaslik = "";
    for (var gx = 0; gx < grupSay; gx++)
    {
        var baslaKolonAdi = gruplar[gx].startColumnName;
        var grupKolonSay = gruplar[gx].numberOfColumns;
        var grupAdi = gruplar[gx].titleText.replace(/<\/?[^>]+(>|$)/g, "");
        //alert(grupAdi);
        var baslaKolon = kolonBul(grid, baslaKolonAdi);
        //alert(baslaKolon+"-"+grupKolonSay+"-"+grupAdi);
        grupBaslik = grupBasliginaEkle(grupBaslik, ilkKolon, baslaKolon, grupAdi, grupKolonSay);
        //alert(grupBaslik);
        ilkKolon = baslaKolon + grupKolonSay;
    }

    grupBaslik = grupBaslik.slice(0, -1);
    return grupBaslik;
}

function kolonBul(grid, kolon)
{
    var columns = grid.jqGrid("getGridParam", "colModel");
    var colNames = [];
    var i = 0;
    for (var cx = 0;cx<columns.length;cx++)
    {
        if (columns[cx].name == kolon)
        {
           return cx   
        }
    }

    return -1;

}

function grupBasliginaEkle(gb, ilkKolon, baslaKolon, grupAdi, grupKolonSay)
{
    //alert(ilkKolon+"-"+baslaKolon+"-"+grupKolonSay);
    for (var ix=ilkKolon+1;ix<baslaKolon;ix++)
    {
        gb += ' '+';';
    }
    var solBos = Math.floor((grupKolonSay - 1) / 2);
    var sagBos = grupKolonSay - 1 - solBos;
    //alert(solBos + "-" + sagBos);
    for (var ix = 1; ix <= solBos; ix++)
    {
        gb += ''+';';
    }
    gb += grupAdi + ";";
    for (var ix = 1; ix <= sagBos; ix++) {
        gb += ' '+';';
    }

    return gb;
}

/*  Nasıl Kullanılacağını bulamadık  */
/**
 * ExcellentExport.
 * A client side Javascript export to Excel.
 *
 * @author: Jordi Burgos (jordiburgos@gmail.com)
 *
 * Based on:
 * https://gist.github.com/insin/1031969
 * http://jsfiddle.net/insin/cmewv/
 *
 * CSV: http://en.wikipedia.org/wiki/Comma-separated_values
 */

/*
 * Base64 encoder/decoder from: http://jsperf.com/base64-optimized
 */

/*jslint browser: true, bitwise: true, plusplus: true, vars: true, white: true */

var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=';
var fromCharCode = String.fromCharCode;
var INVALID_CHARACTER_ERR = (function () {
    "use strict";
    // fabricate a suitable error object
    try {
        document.createElement('$');
    } catch (error) {
        return error;
    }
}());

// encoder
if (!window.btoa) {
    window.btoa = function (string) {
        "use strict";
        var a, b, b1, b2, b3, b4, c, i = 0, len = string.length, max = Math.max, result = '';

        while (i < len) {
            a = string.charCodeAt(i++) || 0;
            b = string.charCodeAt(i++) || 0;
            c = string.charCodeAt(i++) || 0;

            if (max(a, b, c) > 0xFF) {
                throw INVALID_CHARACTER_ERR;
            }

            b1 = (a >> 2) & 0x3F;
            b2 = ((a & 0x3) << 4) | ((b >> 4) & 0xF);
            b3 = ((b & 0xF) << 2) | ((c >> 6) & 0x3);
            b4 = c & 0x3F;

            if (!b) {
                b3 = b4 = 64;
            } else if (!c) {
                b4 = 64;
            }
            result += characters.charAt(b1) + characters.charAt(b2) + characters.charAt(b3) + characters.charAt(b4);
        }
        return result;
    };
}

// decoder
if (!window.atob) {
    window.atob = function (string) {
        "use strict";
        string = string.replace(new RegExp("=+$"), '');
        var a, b, b1, b2, b3, b4, c, i = 0, len = string.length, chars = [];

        if (len % 4 === 1) {
            throw INVALID_CHARACTER_ERR;
        }

        while (i < len) {
            b1 = characters.indexOf(string.charAt(i++));
            b2 = characters.indexOf(string.charAt(i++));
            b3 = characters.indexOf(string.charAt(i++));
            b4 = characters.indexOf(string.charAt(i++));

            a = ((b1 & 0x3F) << 2) | ((b2 >> 4) & 0x3);
            b = ((b2 & 0xF) << 4) | ((b3 >> 2) & 0xF);
            c = ((b3 & 0x3) << 6) | (b4 & 0x3F);

            chars.push(fromCharCode(a));
            b && chars.push(fromCharCode(b));
            c && chars.push(fromCharCode(c));
        }
        return chars.join('');
    };
}


ExcellentExport = (function () {
    "use strict";
    var version = "1.3";
    var csvSeparator = ',';
    var uri = { excel: 'data:application/vnd.ms-excel;base64,', csv: 'data:application/csv;base64,' };
    var template = { excel: '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><meta http-equiv="Content-Type" content="text/html; charset=UTF-8"><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>' };
    var csvDelimiter = ",";
    var csvNewLine = "\r\n";
    var base64 = function (s) {
        return window.btoa(window.unescape(encodeURIComponent(s)));
    };
    var format = function (s, c) {
        return s.replace(new RegExp("{(\\w+)}", "g"), function (m, p) {
            return c[p];
        });
    };

    var get = function (element) {
        if (!element.nodeType) {
            return document.getElementById(element);
        }
        return element;
    };

    var fixCSVField = function (value) {
        var fixedValue = value;
        var addQuotes = (value.indexOf(csvDelimiter) !== -1) || (value.indexOf('\r') !== -1) || (value.indexOf('\n') !== -1);
        var replaceDoubleQuotes = (value.indexOf('"') !== -1);

        if (replaceDoubleQuotes) {
            fixedValue = fixedValue.replace(/"/g, '""');
        }
        if (addQuotes || replaceDoubleQuotes) {
            fixedValue = '"' + fixedValue + '"';
        }
        return fixedValue;
    };

    var tableToCSV = function (table) {
        var data = "";
        var i, j, row, col;
        for (i = 0; i < table.rows.length; i++) {
            row = table.rows[i];
            for (j = 0; j < row.cells.length; j++) {
                col = row.cells[j];
                data = data + (j ? csvDelimiter : '') + fixCSVField(col.textContent.trim());
            }
            data = data + csvNewLine;
        }
        return data;
    };

    var ee = {
        /** @expose */
        excel: function (anchor, table, name) {
            table = get(table);
            var ctx = { worksheet: name || 'Worksheet', table: table.innerHTML };
            var hrefvalue = uri.excel + base64(format(template.excel, ctx));
            anchor.href = hrefvalue;
            // Return true to allow the link to work
            return true;
        },
        /** @expose */
        csv: function (anchor, table, delimiter, newLine) {
            if (delimiter !== undefined && delimiter) {
                csvDelimiter = delimiter;
            }
            if (newLine !== undefined && newLine) {
                csvNewLine = newLine;
            }
            table = get(table);
            var csvData = tableToCSV(table);
            var hrefvalue = uri.csv + base64(csvData);
            anchor.href = hrefvalue;
            return true;
        }
    };

    return ee;
}());
