// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace Auth.RolesToPermission
{
    public enum Permissions : short
    {
        NotSet = 0,

        [Display(GroupName = "Demo", Name = "Demo", Description = "Demo of using a Permission")]
        DemoPermission = 10,

        //This is a special Permission used by the SuperAdmin user. 
        //A user who has this permission has access to every feature.
        [Display(GroupName = "SuperAdmin", Name = "AccessAll", Description = "This allows the user to access every feature")]
        AccessAll = short.MaxValue
    }
}