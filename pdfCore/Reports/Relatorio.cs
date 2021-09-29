using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace pdfCore.Reports
{
    public class Relatorio
    {
        public void gerarRelatorio()
        {
            //Configuração do documento PDF
            var pxPorMm = 72 / 25.2f;
            var pdf = new Document(PageSize.A4, 15 * pxPorMm, 15 * pxPorMm, 15 * pxPorMm, 20 * pxPorMm);
            var nomeArquivo = $"Relatórios{DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss")}.pdf";
            var arquivo = new FileStream(nomeArquivo, FileMode.Create);
            var writer = PdfWriter.GetInstance(pdf, arquivo); //associa o documento ao arquivo pdf
            pdf.Open(); //inicializa pdf, permite que o pdf receba informações.


            //Configuração do documento PDF
            var fonteBase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);

            //adição do titulo
            var fonteParagrafo = new Font(fonteBase, 32, Font.NORMAL, BaseColor.Black);
            var titulo = new Paragraph("Relatório de Alguma Coisa\n\n", fonteParagrafo);
            titulo.Alignment = Element.ALIGN_LEFT;
            pdf.Add(titulo);



            //adição de imagem
            var caminhoImagem = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Imagens\\LogoDay1.jpg");
            if (File.Exists(caminhoImagem))
            {
                Image logo = Image.GetInstance(caminhoImagem);
                float razaoAlturaLargura = logo.Width / logo.Height;
                float alturaLogo = 32;
                float larguraLogo = alturaLogo * razaoAlturaLargura;
                logo.ScaleToFit(larguraLogo, alturaLogo);
                var margemEsquerda = pdf.PageSize.Height - pdf.TopMargin - 54;
                var margemTopo = pdf.PageSize.Height - pdf.TopMargin - 54;
                logo.SetAbsolutePosition(margemEsquerda, margemTopo);
                writer.DirectContent.AddImage(logo, false); //add a imagem ao documento. false - para não seguir o fluxo da page: position abesolute
            }

            pdf.Close();
            arquivo.Close();

            //abre o pdf no visualizador padrão

            var caminhoPdf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nomeArquivo);
            if (File.Exists(caminhoPdf))
            {
                Process.Start(new ProcessStartInfo()
                {
                    Arguments = $"/c start {caminhoPdf}",
                    FileName = "cmd.exe",
                    CreateNoWindow = true
                });
            }



        }
    }
}
