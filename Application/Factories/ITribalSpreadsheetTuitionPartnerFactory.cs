﻿using Application.Extraction;
using Domain;

namespace Application.Factories;

public interface ITribalSpreadsheetTuitionPartnerFactory
{
    TuitionPartner GetTuitionPartner(ISpreadsheetExtractor spreadsheetExtractor, string filename, IList<Region> regions);
}