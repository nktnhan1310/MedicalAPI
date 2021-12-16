﻿using Medical.Entities;
using Medical.Interface.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Interface
{
    public interface IUserPregnancyService : IDomainService<UserPregnancies, SearchUserPregnancy>
    {
    }
}
