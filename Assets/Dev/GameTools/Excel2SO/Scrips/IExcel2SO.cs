using System.Collections.Generic;

public interface IExcel2SO
{
    void FromExcelToSO(List<List<ExcelGenericRow>> parsedSheets);
}
