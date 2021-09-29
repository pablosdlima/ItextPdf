using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using pdfCore.Models;
using System;
using System.Collections.Generic;
using System.IO; // lib que permite a entrada e saída de arquivos.
using System.Linq;
using System.Threading.Tasks;

namespace pdfCore.Reports
{
    public class StudentReport
    {
        private  IWebHostEnvironment _webHostEnvironment;
        public StudentReport(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region Declaration
        int _maxColumn = 3;
        int contador = 1;
        Document _doc;
        Font _estiloFonte;
        PdfPTable _tabelaPdf = new(3);
        PdfPCell _celulaTabela;
        MemoryStream _memoryStream = new();

        List<Student> _students = new();
        #endregion

        public byte[] Report(List<Student> students)
        {
            _students = students;

            if(_students.Count > 1)
            {
                int totalPaginas = 1;
                int totalLinhas = _students.Count;

                totalPaginas = (totalLinhas % 30 >= 1) ? totalLinhas / 30 + 1 : totalLinhas / 30; //quantidade de registros dividido pela quantidade de linhas por  pagina
               
                _doc = new Document(); //instancia de documento
                _doc.SetPageSize(PageSize.A4); //define o tipo de pagina
                _doc.SetMargins(30f, 30f, 20f, 30f); //espaçamento de margens do documento
                _tabelaPdf.WidthPercentage = 100;

                _tabelaPdf.WidthPercentage = 100;
                _tabelaPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                _estiloFonte = FontFactory.GetFont("Tahoma", 8f, 1);

                PdfWriter docWrite = PdfWriter.GetInstance(_doc, _memoryStream); //cria documento em branco

                _doc.Open();  //abre documento, permitindo que receba as informações do pdf

                float[] sizes = new float[_maxColumn];

                for (var i = 0; i < _maxColumn; i++)
                {
                    if (i == 0) sizes[i] = 20; else sizes[i] = 100;
                }

                _tabelaPdf.SetWidths(sizes);

                //adição do titulo
                var fonteBase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                var fonteParagrafo = new Font(fonteBase, 10, Font.NORMAL, BaseColor.Black);
                var titulo = new Paragraph($"Total de Registros: {students.Count}\n", fonteParagrafo);
                titulo.Alignment = Element.ALIGN_RIGHT;
                _doc.Add(titulo);

                this.Cabecalho();
                this.LinhaVazia(1);
                this.corpoPage();
                this.Rodape(docWrite, totalPaginas);

                _tabelaPdf.HeaderRows = 2;
                _doc.Add(_tabelaPdf);

                _doc.Close();
            }
            return _memoryStream.ToArray();
        }
        private void Cabecalho() {

            _celulaTabela = new PdfPCell(this.AddLogo());
            _celulaTabela.Colspan = 1;
            _celulaTabela.BorderWidth = 0;
            _tabelaPdf.AddCell(_celulaTabela);

            _celulaTabela = new PdfPCell(this.TituloPage());
            _celulaTabela.Colspan = _maxColumn - 1;
            _celulaTabela.BorderWidth = 0;
            _tabelaPdf.AddCell(_celulaTabela);

            _tabelaPdf.CompleteRow();
        }

        private void Rodape(PdfWriter docWrite, int totalPage)
        {
            docWrite.PageEvent = new EventoPage(totalPage);
        }

        private PdfPTable AddLogo()
        {
            int maxColumn = 1;
            PdfPTable pdfPTable = new(maxColumn);
            string path = _webHostEnvironment.WebRootPath + "/Imagens";

            string imgCombine = Path.Combine(path, "LogoDay1.jpg");
            Image img = Image.GetInstance(imgCombine);
            img.ScalePercent(25f); //tamanho imagem
            //img.ScaleAbsolute(300f, 100f); //permite achatar ou não img
            //img.SetAbsolutePosition(_doc.PageSize.Width - 90f - 10f, _doc.PageSize.Height - 20f -80f);

            _celulaTabela = new(img);
            _celulaTabela.Colspan = maxColumn;
            _celulaTabela.HorizontalAlignment = Element.ALIGN_LEFT;
            _celulaTabela.Border = 0;
            _celulaTabela.ExtraParagraphSpace = 0;
            pdfPTable.AddCell(_celulaTabela);

            pdfPTable.CompleteRow();

            return pdfPTable;
        }
        private PdfPTable TituloPage() //titulo da pagina
        {
            int maxColumn = 1;
            PdfPTable pdfPTable = new(maxColumn);

            CriarCelulaTexto("Relatório de Teste");

            pdfPTable.AddCell(_celulaTabela); //add a celula a tabela
            pdfPTable.CompleteRow();

            return pdfPTable;
        }

        private void CriarCelulaTexto(string parTitulo)
        {
            _estiloFonte = FontFactory.GetFont("Tahoma", 14f, BaseColor.DarkGray);
            _celulaTabela = new PdfPCell(new Phrase(parTitulo, _estiloFonte)); //add conteúdo a celula da tabela.
            _celulaTabela.HorizontalAlignment = Element.ALIGN_CENTER;
            _celulaTabela.PaddingTop = 80f;
            _celulaTabela.PaddingRight = 50f;
            _celulaTabela.ExtraParagraphSpace = 0;
            _celulaTabela.BorderWidth = 0;
        }

        private void LinhaVazia(int nCount) //pula linha
        {
            for(int i = 1; i <= nCount; i++)
            {
                _celulaTabela = new PdfPCell(new Phrase(" ", _estiloFonte));
                _celulaTabela.Colspan = _maxColumn;
                _celulaTabela.Border = 0;
                _celulaTabela.ExtraParagraphSpace = 0;
                _tabelaPdf.AddCell(_celulaTabela);
                _tabelaPdf.CompleteRow();
            }
        }

        private void corpoPage() //Corpo da pagina
        {
            var fontStyleBold = FontFactory.GetFont("Tahoma", 12f, 1);
            CriarColuna(fontStyleBold, "Id");
            CriarColuna(fontStyleBold, "Nome");
            CriarColuna(fontStyleBold, "SobreNome");
            _tabelaPdf.CompleteRow(); //necessário para fechar linha

            foreach (var student in _students)
            {
                CriarColunaInfo(fontStyleBold, student.Id.ToString(), contador);
                CriarColunaInfo(fontStyleBold, student.Name, contador);
                CriarColunaInfo(fontStyleBold, student.Address, contador);
                _tabelaPdf.CompleteRow();
                contador++;
            }
        }

        private void CriarColunaInfo(Font fontStyleBold, string student, int contador)
        {
            _celulaTabela = new PdfPCell(new Phrase(student, fontStyleBold));
            _celulaTabela.HorizontalAlignment = Element.ALIGN_CENTER;
            _celulaTabela.VerticalAlignment = Element.ALIGN_MIDDLE;
            _celulaTabela.Padding = 5;
            _ = contador % 2 == 0 ? (_celulaTabela.BackgroundColor = BaseColor.LightGray) : (_celulaTabela.BackgroundColor = BaseColor.White);
            _tabelaPdf.AddCell(_celulaTabela);
        }

        private void CriarColuna(Font fontStyleBold, string nomeColuna)
        {
            _celulaTabela = new PdfPCell(new Phrase(nomeColuna, fontStyleBold));
            _celulaTabela.HorizontalAlignment = Element.ALIGN_CENTER;
            _celulaTabela.VerticalAlignment = Element.ALIGN_MIDDLE;
            _celulaTabela.Padding = 5;
            _celulaTabela.BackgroundColor = BaseColor.Gray;
            _celulaTabela.BorderWidthBottom = 2f;
            _celulaTabela.Border = 0;
            _tabelaPdf.AddCell(_celulaTabela);
        }
    }//10:12 // https://www.youtube.com/watch?v=SLNAKXd0NvM
}