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

namespace LabBenchStudios.Pdt.Common
{
    public class ResourceNameContainer
    {
        public string ProductPrefix { private set; get; } = ConfigConst.PRODUCT_NAME;
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

        public IotDataContext DataContext { private set; get; } = null;

        private string fullResourceName = "Not-Set";

        public ResourceNameContainer()
        {
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
            return this.fullResourceName;
        }


        // private methods

        private void InitFullResourceName()
        {
            this.fullResourceName = this.ProductPrefix + ConfigConst.RESOURCE_SEPARATOR + this.DeviceName + ConfigConst.RESOURCE_SEPARATOR + this.ResourceTypeName;
        }
    }
}
