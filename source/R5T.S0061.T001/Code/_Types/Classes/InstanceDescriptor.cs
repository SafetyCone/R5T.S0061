using System;


namespace R5T.S0061.T001
{
    public class InstanceDescriptor
    {
        public string InstanceVariety { get; set; }
        public string ProjectFilePath { get; set; }
        public string IdentityName { get; set; }
        public string ParameterNamedIdentityName { get; set; }
        public string DescriptionXml { get; set; }


        public override string ToString()
        {
            var representation = this.IdentityName;
            return representation;
        }
    }
}


namespace R5T.S0061.T001.N001
{
    public class InstanceDescriptor
    {
        public string InstanceVariety { get; set; }
        public string IdentityName { get; set; }
        public string ParameterNamedIdentityName { get; set; }
        public string DescriptionXml { get; set; }


        public override string ToString()
        {
            var representation = this.IdentityName;
            return representation;
        }
    }
}