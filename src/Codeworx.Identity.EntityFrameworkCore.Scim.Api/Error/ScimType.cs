namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Error
{
    public enum ScimType
    {
        /// <summary>
        /// | invalidFilter | The specified filter syntax    | GET (Section     |
        /// |               | was invalid (does not comply   | 3.4.2), POST     |
        /// |               | with Figure 1), or the         | (Search -        |
        /// |               | specified attribute and filter | Section 3.4.3),  |
        /// |               | comparison combination is not  | PATCH (Path      |
        /// |               | supported.                     | Filter - Section |
        /// |               |                                | 3.5.2)           |
        /// |               |                                |                  |.
        /// </summary>
        InvalidFilter = 0,

        /// <summary>
        /// | tooMany       | The specified filter yields    | GET (Section     |
        /// |               | many more results than the     | 3.4.2), POST     |
        /// |               | server is willing to calculate | (Search -        |
        /// |               | or process.  For example, a    | Section 3.4.3)   |
        /// |               | filter such as "(userName pr)" |                  |
        /// |               | by itself would return all     |                  |
        /// |               | entries with a "userName" and  |                  |
        /// |               | MAY not be acceptable to the   |                  |
        /// |               | service provider.              |                  |
        /// |               |                                |                  |.
        /// </summary>
        TooMany = 1,

        /// <summary>
        /// | uniqueness    | One or more of the attribute   | POST (Create -   |
        /// |               | values are already in use or   | Section 3.3),    |
        /// |               | are reserved.                  | PUT (Section     |
        /// |               |                                | 3.5.1), PATCH    |
        /// |               |                                | (Section 3.5.2)  |
        /// |               |                                |                  |.
        /// </summary>
        Uniqueness = 2,

        /// <summary>
        /// | mutability    | The attempted modification is  | PUT (Section     |
        /// |               | not compatible with the target | 3.5.1), PATCH    |
        /// |               | attribute's mutability or      | (Section 3.5.2)  |
        /// |               | current state (e.g.,           |                  |
        /// |               | modification of an "immutable" |                  |
        /// |               | attribute with an existing     |                  |
        /// |               | value).                        |                  |
        /// |               |                                |                  |.
        /// </summary>
        Mutability = 3,

        /// <summary>
        /// | invalidSyntax | The request body message       | POST (Search -   |
        /// |               | structure was invalid or did   | Section 3.4.3,   |
        /// |               | not conform to the request     | Create - Section |
        /// |               | schema.                        | 3.3, Bulk -      |
        /// |               |                                | Section 3.7),    |
        /// |               |                                | PUT (Section     |
        /// |               |                                | 3.5.1)           |
        /// |               |                                |                  |.
        /// </summary>
        InvalidSyntax = 4,

        /// <summary>
        /// | invalidPath   | The "path" attribute was       | PATCH (Section   |
        /// |               | invalid or malformed (see      | 3.5.2)           |
        /// |               | Figure 7).                     |                  |
        /// |               |                                |                  |.
        /// </summary>
        InvalidPath = 5,

        /// <summary>
        /// | noTarget      | The specified "path" did not   | PATCH (Section   |
        /// |               | yield an attribute or          | 3.5.2)           |
        /// |               | attribute value that could be  |                  |
        /// |               | operated on.  This occurs when |                  |
        /// |               | the specified "path" value     |                  |
        /// |               | contains a filter that yields  |                  |
        /// |               | no match.                      |                  |
        /// |               |                                |                  |.
        /// </summary>
        NoTarget = 6,

        /// <summary>
        /// | invalidValue  | A required value was missing,  | GET (Section     |
        /// |               | or the value specified was not | 3.4.2), POST     |
        /// |               | compatible with the operation  | (Create -        |
        /// |               | or attribute type (see Section | Section 3.3,     |
        /// |               | 2.2 of [RFC7643]), or resource | Query - Section  |
        /// |               | schema (see Section 4 of       | 3.4.3), PUT      |
        /// |               | [RFC7643]).                    | (Section 3.5.1), |
        /// |               |                                | PATCH (Section   |
        /// |               |                                | 3.5.2)           |
        /// |               |                                |                  |.
        /// </summary>
        InvalidValue = 7,

        /// <summary>
        /// | invalidVers   | The specified SCIM protocol    | GET (Section     |
        /// |               | version is not supported (see  | 3.4.2), POST     |
        /// |               | Section 3.13).                 | (ALL), PUT       |
        /// |               |                                | (Section 3.5.1), |
        /// |               |                                | PATCH (Section   |
        /// |               |                                | 3.5.2), DELETE   |
        /// |               |                                | (Section 3.6)    |
        /// |               |                                |                  |.
        /// </summary>
        InvalidVers = 8,

        /// <summary>
        /// | sensitive     | The specified request cannot   | GET (Section     |
        /// |               | be completed, due to the       | 3.4.2)           |
        /// |               | passing of sensitive (e.g.,    |                  |
        /// |               | personal) information in a     |                  |
        /// |               | request URI.  For example,     |                  |
        /// |               | personal information SHALL NOT |                  |
        /// |               | be transmitted over request    |                  |
        /// |               | URIs.  See Section 7.5.2.      |                  |.
        /// </summary>
        Sensitive = 9,
    }
}
