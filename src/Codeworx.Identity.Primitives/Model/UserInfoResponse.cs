using System.Runtime.Serialization;

namespace Codeworx.Identity.Model
{
    [DataContract]
    public class UserInfoResponse
    {
        [DataMember(Order = 1, Name = "sub")]
        public string Subject { get; set; }

        [DataMember(Order = 2, Name = "name")]
        public string Name { get; set; }

        [DataMember(Order = 3, Name = "given_name")]
        public string GivenName { get; set; }

        [DataMember(Order = 4, Name = "family_name")]
        public string FamilyName { get; set; }

        [DataMember(Order = 5, Name = "middle_name")]
        public string MiddleName { get; set; }

        [DataMember(Order = 6, Name = "nickname")]
        public string NickName { get; set; }

        [DataMember(Order = 7, Name = "preferred_username")]
        public string PreferredUsername { get; set; }

        [DataMember(Order = 8, Name = "profile")]
        public string Profile { get; set; }

        [DataMember(Order = 9, Name = "picture")]
        public string Picture { get; set; }

        [DataMember(Order = 10, Name = "website")]
        public string WebSite { get; set; }

        [DataMember(Order = 11, Name = "email")]
        public string EMail { get; set; }

        [DataMember(Order = 12, Name = "email_verified")]
        public bool VerifiedEmail { get; set; }

        [DataMember(Order = 13, Name = "gender")]
        public string Gender { get; set; }

        [DataMember(Order = 14, Name = "birthdate")]
        public string YearOfBirthday { get; set; }

        [DataMember(Order = 15, Name = "zoneinfo")]
        public string ZoneInfo { get; set; }

        [DataMember(Order = 16, Name = "locale")]
        public string Localization { get; set; }

        [DataMember(Order = 17, Name = "phone_number")]
        public string Phone { get; set; }

        [DataMember(Order = 18, Name = "phone_number_verified")]
        public bool VerifiedPhone { get; set; }
    }
}
