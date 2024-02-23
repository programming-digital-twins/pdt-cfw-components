/**
 * MIT License
 * 
 * Copyright (c) 2024 Andrew D. King
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using LabBenchStudios.Pdt.Data;
using NodaTime;

namespace LabBenchStudios.Pdt.Common
{
    public class ResourceNameContainer
    {
        public string ProductPrefix { set; get; } = ConfigConst.PRODUCT_NAME;
        public string DeviceName { private set; get; } = "Not-Set";
        public string ResourceTypeName { private set; get; } = "Not-Set";
        public string FullTypeName { private set; get; } = "Not-Set";
        public string ResourceSubTypeName { private set; get; } = "Not-Set";
        public int TypeID { private set; get; } = ConfigConst.DEFAULT_TYPE_ID;
        public int TypeCategoryID { private set; get; } = ConfigConst.DEFAULT_TYPE_CATEGORY_ID;
        public bool IsActuationResource { private set; get; } = false;
        public bool IsConnStateResource { private set; get; } = false;
        public bool IsMediaResource { private set; get; } = false;
        public bool IsMessageResource { private set; get; } = false;
        public bool IsSensingResource { private set; get; } = false;
        public bool IsSystemResource { private set; get; } = false;
        public bool IsSubscription { private set; get; } = false;

        public string PersistenceName { set; get; } = null;

        public IotDataContext DataContext {
            get => this._dataContext;
            set {
                if ((value != null) && (value is IotDataContext))
                {
                    this._dataContext = value;
                    this.DeviceName = this._dataContext.GetDeviceID();
                    this.TypeID = this._dataContext.GetTypeID();
                    this.TypeCategoryID = this._dataContext.GetTypeCategoryID();

                    if (this._dataContext is ActuatorData)
                    {
                        this.IsActuationResource = true;
                    }
                    else if (this._dataContext is ConnectionStateData)
                    {
                        this.IsConnStateResource = true;
                    } else if (this._dataContext is SensorData)
                    {
                        this.IsSensingResource = true;
                    } else if (this._dataContext is SystemPerformanceData)
                    {
                        this.IsSystemResource = true;
                    }

                    this.InitFullResourceName();
                }
            }
        }

        private IotDataContext _dataContext = null;

        private string _fullResourceName = "Not-Set";

        public ResourceNameContainer()
        {
            this.InitFullResourceName();
        }

        public ResourceNameContainer(string deviceName, string resourceTypeName)
        {
            DeviceName = deviceName;
            ResourceTypeName = resourceTypeName;

            this.InitFullResourceName();
        }

        public ResourceNameContainer(string deviceName, string resourceTypeName, IotDataContext data)
        {
            DeviceName = deviceName;
            ResourceTypeName = resourceTypeName;
            DataContext = data;

            TypeID = data.GetDeviceType();
            TypeCategoryID = data.GetDeviceCategory();

            this.InitFullResourceName();
        }

        // public methods

        public string GetFullResourceName()
        {
            return this._fullResourceName;
        }


        // private methods

        private void InitFullResourceName()
        {
            this._fullResourceName = this.ProductPrefix + ConfigConst.RESOURCE_SEPARATOR + this.DeviceName + ConfigConst.RESOURCE_SEPARATOR + this.ResourceTypeName;
        }
    }
}
