namespace Pilgrimage_Of_Embers.Global.Interfaces
{
    interface IUnload
    {
        void Unload();
        bool IsUnloading();
        bool IsUnloaded();

        /* <--- Copy-Paste Code for implementing this interface properly! --->
        
        protected bool isUnloading = false;
        bool isUnloaded = false;
        public virtual void Unload()
        {
           isUnloading = true;
           
           //unload stuff between here!
         
           isUnloaded = true;
        }
        public bool IsUnloading() //Tell whoever is listening that we have begun unloading.
        {
            return isUnloading;
        }
        public bool IsUnloaded() //Tell whoever is listening that we have finished unloading.
        {
            return isUnloaded;
        }
         
        */
    }
}
