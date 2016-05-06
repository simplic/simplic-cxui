using Simplic.CXUI.BuildTask;
using Simplic.CXUI.BuildTask.ViewModel;

namespace Simplic.CXUI.JsonViewModel
{
    /// <summary>
    /// Build viewmodels which bases on a json configuration
    /// </summary>
    public class JsonViewModelBuildTask : BuildViewModelTask
    {
        /// <summary>
        /// Generate a viewmodel which is designed in json
        /// </summary>
        /// <param name="code">Json code which descripes the viewmodel</param>
        /// <returns>Meta viewmodel instance or throws an expcetion</returns>
        public override MetaViewModel GenerateMetaViewModel(string code)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<MetaViewModel>(code);
        }
    }
}
