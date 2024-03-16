using Data;

namespace DesktopPart.Service;

public interface IExelService
{
    List<InputParameters> ImportFromExcel();
    void ExportApproximationToExel(ManyParameters userParameters, ManyParameters aproximationParameters);
    void ExportInputDataToExel(ManyParameters userParameters);
}