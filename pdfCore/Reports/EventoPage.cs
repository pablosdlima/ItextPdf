using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pdfCore.Reports
{
    public class EventoPage : PdfPageEventHelper
    {
        private int _totalPaginas = 1;
        private BaseFont fonteBaseRodape { get; set; }
        private Font fonteRodape { get; set; }

        public EventoPage(int totalPaginas)
        {
            fonteBaseRodape = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
            fonteRodape = new Font(fonteBaseRodape, 8f, Font.NORMAL, BaseColor.Black);
            _totalPaginas = totalPaginas;
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            GeracaoRelatorio(writer, document);
            AdicionarNumeroPage(writer, document);
        }

        private void GeracaoRelatorio(PdfWriter writer, Document document)
        {
            var textoMomentoGeracao = $"Gerado: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}";

           writer.DirectContent.BeginText();
           writer.DirectContent.SetFontAndSize(fonteRodape.BaseFont, fonteRodape.Size);
           writer.DirectContent.SetTextMatrix(document.LeftMargin, document.BottomMargin * 0.85f);
           writer.DirectContent.ShowText(textoMomentoGeracao);
           writer.DirectContent.EndText();
        }

        private void AdicionarNumeroPage(PdfWriter writer, Document document)
        {
            int paginaAtual = writer.PageNumber;
            var textoPaginacao = $"Página {paginaAtual} de {_totalPaginas}";

            float larguraTextoPaginacao = fonteBaseRodape.GetWidthPoint(textoPaginacao, fonteRodape.Size);
            var tamanhoPagina = document.PageSize;

            writer.DirectContent.BeginText();
            writer.DirectContent.SetFontAndSize(fonteRodape.BaseFont, fonteRodape.Size);
            writer.DirectContent.SetTextMatrix(tamanhoPagina.Width - document.RightMargin - larguraTextoPaginacao, document.BottomMargin * 0.85f);
            writer.DirectContent.ShowText(textoPaginacao);
            writer.DirectContent.EndText();
        }
    }
}
