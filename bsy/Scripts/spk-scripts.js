/* Author: Onur Kocer 
   Yeni yazılan scriptleri buraya ekleyelim lütfen 
*/

// val: sayısal değer
function SayiTOgrupluVirgullu(val)
{
    return val.toLocaleString('it-IT');
}

// val: noktalı gruplu ya da grupsuz virgüllü kesir içeren string
function GrupluVirgulluKesirTOsayi(gk)
{
    return parseFloat(GrupsuzNoktaliKesir(gk));
}

function GrupluVirgulluKesirTOgrupluVirgulluKesir(gk0)
{
    gk = GrupsuzNoktaliKesir(gk0);
    if (gk == '')
        return '';

    x = gk.split('.');
    if (x.length > 2)
        return gk0.substr(0,gk0.length-1);
    x2 = x.length > 1 ? x[1] : '';
    // x[0] tam sayı olduğu için virgül içermiyor
    var grupluTamKisim = SayiTOgrupluVirgullu(GrupluVirgulluKesirTOsayi(x[0]));
    return x.length == 1 ? grupluTamKisim : grupluTamKisim + ',' + x2;
}

function GrupluVirgulluKesirTOgrupluVirgullu0Yok(gk0)
{
    //alert("gelen:"+gk0);
    gk = GrupsuzNoktaliKesir(gk0);
    //alert("grupsuznoktalikesir:" + gk);

    if (gk == '')
        return '';

    x = gk.split('.');
    if (x.length > 2)
        return gk0;
    x2 = x.length > 1 ? x[1] : '0';
    var y2 = parseFloat(x2);
    // x[0] tam sayı olduğu için virgül içermiyor
    //alert("xo:" + x[0]);
    var grupluTamKisim = SayiTOgrupluVirgullu(GrupluVirgulluKesirTOsayi(x[0]));
    //alert("gruplutamkısım:" + grupluTamKisim);
    return y2 == 0 ? grupluTamKisim : grupluTamKisim + ',' + x2;

}

function GrupsuzNoktaliKesir(gk)
{
    gk = gk.replace(' ','');
    gk = gk.replace('.', ''); gk = gk.replace('.', ''); gk = gk.replace('.', '');
    gk = gk.replace('.', ''); gk = gk.replace('.', ''); gk = gk.replace('.', '');
    gk = gk.replace(',', '.');
    gk = gk.replace(',', '.');

    return gk;
}

//
function formatAsCurrency(Num)
{
    var sayi = grupluKurusluSayiTOsayi(Num);
    return grupluKurusluSayi(sayi);

    /*
    Num += '';
    Num = Num.replace('.', ''); Num = Num.replace('.', ''); Num = Num.replace('.', '');
    Num = Num.replace('.', ''); Num = Num.replace('.', ''); Num = Num.replace('.', '');

    
    x = Num.split(',');
    x1 = x[0];
    x2 = x.length > 1 ? ',' + x[1] : '';
    //alert(Num+"=>"+x.length);
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1))
        x1 = x1.replace(rgx, '$1' + '.' + '$2');
    return x1 + x2;
    */
}

function formatAsCurrencyNumber(Num)
{
    return GrupluVirgulluKesirTOgrupluVirgulluKesir(Num);

    /*
    Num += '';
    //alert(Num);
    Num = Num.replace('.', ''); Num = Num.replace('.', ''); Num = Num.replace('.', '');
    Num = Num.replace('.', ''); Num = Num.replace('.', ''); Num = Num.replace('.', '');
    //alert(Num);
    var dgr=grupluKurusluSayi(Num);
    //alert(dgr);
    return dgr;

    x = Num.split(',');
    x1 = x[0];
    x2 = x.length > 1 ? ',' + x[1] : '';
    //alert(x1 + "-" + x2);
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1))
        x1 = x1.replace(rgx, '$1' + '.' + '$2');
    return x1;
    */
}


function getNextElement(field) {//bir sonraki text submit ve button elemente geçmek için
    var form = field.form;
    for (var e = 0; e < form.elements.length; e++) {
        if (field == form.elements[e]) {
            break;
        }
    }
    e++;
    while (form.elements[e % form.elements.length].type != "text" && form.elements[e % form.elements.length].type != "submit"
    && form.elements[e % form.elements.length].type != "button") {
        e++;
    }
    return form.elements[e % form.elements.length]; 
}

function allowOnlyNumbers(evt)
{
    var charCode = (evt.which) ? evt.which : event.keyCode

    //alert(charCode);

    if (charCode == 13) {//eğer basılan tuş enter ise
        var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
        if ((node.type == "text")) {//eğer enter a bir text tipinde element te basıldı ise
            getNextElement(node).focus();
            return false;
        } 
    }

    //alert("geldi");

    if (charCode != 44)
        if (charCode > 31 && (charCode < 48 || charCode > 57))
        {
            if (charCode == 44)
            {
                return true;
            }
            return false;
        }

    return true;
}

function insertThousandsSeparator(val)
{
    return val.replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

function jqGridHeaderCursor(gridID)
{
    var myGrid = $(gridID);

    // fix cursor on non-sortable columns
    var cm = myGrid[0].p.colModel;
    $.each(myGrid[0].grid.headers, function (index, value)
    {
        var cmi = cm[index], colName = cmi.name;
        if (!cmi.sortable && colName !== 'rn' && colName !== 'cb' && colName !== 'subgrid')
        {
            $('div.ui-jqgrid-sortable', value.el).css({ cursor: "default" });
        }
    });
}

function resizeGrid(gridID)
{
    gridParentWidth = $('#gbox_' + gridID).parent().width();
    $('#' + gridID).setGridWidth(gridParentWidth);
}