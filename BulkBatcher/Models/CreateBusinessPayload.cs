using System.Text.Json.Serialization;

namespace BulkBatcher.Models
{
    public class CreateBusinessPayload
    {
        // Required fields
        [JsonPropertyName("external_id")]
        public string ExternalId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        // Optional fields - all nullable
        [JsonPropertyName("tin")]
        public string? Tin { get; set; }

        [JsonPropertyName("address_line_1")]
        public string? AddressLine1 { get; set; }

        [JsonPropertyName("address_line_2")]
        public string? AddressLine2 { get; set; }

        [JsonPropertyName("address_city")]
        public string? AddressCity { get; set; }

        [JsonPropertyName("address_state")]
        public string? AddressState { get; set; }

        [JsonPropertyName("address_country")]
        public string? AddressCountry { get; set; }

        [JsonPropertyName("address_postal_code")]
        public string? AddressPostalCode { get; set; }

        [JsonPropertyName("mobile")]
        public string? Mobile { get; set; }

        [JsonPropertyName("official_website")]
        public string? OfficialWebsite { get; set; }

        [JsonPropertyName("naics_code")]
        public string? NaicsCode { get; set; }

        [JsonPropertyName("naics_title")]
        public string? NaicsTitle { get; set; }

        [JsonPropertyName("mcc_code")]
        [JsonConverter(typeof(NullableInt32JsonConverter))]
        public int? MccCode { get; set; }

        [JsonPropertyName("industry")]
        public string? Industry { get; set; }

        [JsonPropertyName("is_monitoring_enabled")]
        public bool? IsMonitoringEnabled { get; set; }

        [JsonPropertyName("applicant_id")]
        public string? ApplicantId { get; set; }

        [JsonPropertyName("send_invitation")]
        public bool? SendInvitation { get; set; }

        [JsonPropertyName("applicant_first_name")]
        public string? ApplicantFirstName { get; set; }

        [JsonPropertyName("applicant_last_name")]
        public string? ApplicantLastName { get; set; }

        [JsonPropertyName("applicant_email")]
        public string? ApplicantEmail { get; set; }

        [JsonPropertyName("dba1_name")]
        public string? Dba1Name { get; set; }

        [JsonPropertyName("address1_line_1")]
        public string? Address1Line1 { get; set; }

        [JsonPropertyName("address1_apartment")]
        public string? Address1Apartment { get; set; }

        [JsonPropertyName("address1_city")]
        public string? Address1City { get; set; }

        [JsonPropertyName("address1_state")]
        public string? Address1State { get; set; }

        [JsonPropertyName("address1_country")]
        public string? Address1Country { get; set; }

        [JsonPropertyName("address1_postal_code")]
        public string? Address1PostalCode { get; set; }

        [JsonPropertyName("address1_mobile")]
        public string? Address1Mobile { get; set; }

        [JsonPropertyName("year_created")]
        [JsonConverter(typeof(NullableInt32JsonConverter))]
        public int? YearCreated { get; set; }

        [JsonPropertyName("annual_total_income")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? AnnualTotalIncome { get; set; }

        [JsonPropertyName("total_wages")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? TotalWages { get; set; }

        [JsonPropertyName("annual_net_income")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? AnnualNetIncome { get; set; }

        [JsonPropertyName("cost_of_goods_sold")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? CostOfGoodsSold { get; set; }

        [JsonPropertyName("total_liabilities")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? TotalLiabilities { get; set; }

        [JsonPropertyName("total_assets")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? TotalAssets { get; set; }

        [JsonPropertyName("total_equity")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? TotalEquity { get; set; }

        [JsonPropertyName("total_accounts_payable")]
        [JsonConverter(typeof(NullableInt32JsonConverter))]
        public int? TotalAccountsPayable { get; set; }

        [JsonPropertyName("total_accounts_recievable")]
        [JsonConverter(typeof(NullableInt32JsonConverter))]
        public int? TotalAccountsReceivable { get; set; }

        [JsonPropertyName("total_cash_and_cash_equivalents")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? TotalCashAndCashEquivalents { get; set; }

        [JsonPropertyName("total_short_term_investments")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? TotalShortTermInvestments { get; set; }

        [JsonPropertyName("total_current_assets")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? TotalCurrentAssets { get; set; }

        [JsonPropertyName("total_current_liabilities")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? TotalCurrentLiabilities { get; set; }

        [JsonPropertyName("non_current_liablities")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? NonCurrentLiabilities { get; set; }

        [JsonPropertyName("annual_cost_of_goods_sold")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? AnnualCostOfGoodsSold { get; set; }

        [JsonPropertyName("annual_gross_profit")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? AnnualGrossProfit { get; set; }

        [JsonPropertyName("annual_taxes_paid")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? AnnualTaxesPaid { get; set; }

        [JsonPropertyName("annual_interest_expenses")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? AnnualInterestExpenses { get; set; }

        [JsonPropertyName("number_of_employees")]
        [JsonConverter(typeof(NullableInt32JsonConverter))]
        public int? NumberOfEmployees { get; set; }

        [JsonPropertyName("business_type")]
        public string? BusinessType { get; set; }

        [JsonPropertyName("sic_code")]
        public string? SicCode { get; set; }

        [JsonPropertyName("score_retrieval_date")]
        public string? ScoreRetrievalDate { get; set; }

        [JsonPropertyName("business_liens")]
        [JsonConverter(typeof(NullableInt32JsonConverter))]
        public int? BusinessLiens { get; set; }

        [JsonPropertyName("business_liens_file_date")]
        public string? BusinessLiensFileDate { get; set; }

        [JsonPropertyName("business_liens_status")]
        public string? BusinessLiensStatus { get; set; }

        [JsonPropertyName("business_liens_status_date")]
        public string? BusinessLiensStatusDate { get; set; }

        [JsonPropertyName("business_bankruptcies")]
        [JsonConverter(typeof(NullableInt32JsonConverter))]
        public int? BusinessBankruptcies { get; set; }

        [JsonPropertyName("business_bankruptcies_file_date")]
        public string? BusinessBankruptciesFileDate { get; set; }

        [JsonPropertyName("business_bankruptcies_chapter")]
        public string? BusinessBankruptciesChapter { get; set; }

        [JsonPropertyName("business_bankruptcies_voluntary")]
        public string? BusinessBankruptciesVoluntary { get; set; }

        [JsonPropertyName("business_bankruptcies_status")]
        public string? BusinessBankruptciesStatus { get; set; }

        [JsonPropertyName("business_bankruptcies_status_date")]
        public string? BusinessBankruptciesStatusDate { get; set; }

        [JsonPropertyName("business_judgements")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? BusinessJudgements { get; set; }

        [JsonPropertyName("business_judgements_file_date")]
        public string? BusinessJudgementsFileDate { get; set; }

        [JsonPropertyName("business_judgements_status")]
        public string? BusinessJudgementsStatus { get; set; }

        [JsonPropertyName("business_judgements_status_date")]
        public string? BusinessJudgementsStatusDate { get; set; }

        [JsonPropertyName("business_judgements_amount")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? BusinessJudgementsAmount { get; set; }

        [JsonPropertyName("social_review_count")]
        [JsonConverter(typeof(NullableInt32JsonConverter))]
        public int? SocialReviewCount { get; set; }

        [JsonPropertyName("social_review_score")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? SocialReviewScore { get; set; }

        [JsonPropertyName("owner1_title")]
        public string? Owner1Title { get; set; }

        [JsonPropertyName("owner1_first_name")]
        public string? Owner1FirstName { get; set; }

        [JsonPropertyName("owner1_last_name")]
        public string? Owner1LastName { get; set; }

        [JsonPropertyName("owner1_email")]
        public string? Owner1Email { get; set; }

        [JsonPropertyName("owner1_mobile")]
        public string? Owner1Mobile { get; set; }

        [JsonPropertyName("owner1_ssn")]
        public string? Owner1Ssn { get; set; }

        [JsonPropertyName("owner1_dob")]
        public string? Owner1Dob { get; set; }

        [JsonPropertyName("owner1_address_line_1")]
        public string? Owner1AddressLine1 { get; set; }

        [JsonPropertyName("owner1_address_line_2")]
        public string? Owner1AddressLine2 { get; set; }

        [JsonPropertyName("owner1_address_city")]
        public string? Owner1AddressCity { get; set; }

        [JsonPropertyName("owner1_address_state")]
        public string? Owner1AddressState { get; set; }

        [JsonPropertyName("owner1_address_postal")]
        public string? Owner1AddressPostal { get; set; }

        [JsonPropertyName("owner1_address_country")]
        public string? Owner1AddressCountry { get; set; }

        [JsonPropertyName("owner1_owner_type")]
        public string? Owner1OwnerType { get; set; }

        [JsonPropertyName("owner1_ownership_percentage")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? Owner1OwnershipPercentage { get; set; }

        [JsonPropertyName("bank_account_number")]
        public string? BankAccountNumber { get; set; }

        [JsonPropertyName("bank_name")]
        public string? BankName { get; set; }

        [JsonPropertyName("institution_name")]
        public string? InstitutionName { get; set; }

        [JsonPropertyName("bank_routing_number")]
        public string? BankRoutingNumber { get; set; }

        [JsonPropertyName("bank_wire_routing_number")]
        public string? BankWireRoutingNumber { get; set; }

        [JsonPropertyName("bank_official_name")]
        public string? BankOfficialName { get; set; }

        [JsonPropertyName("bank_account_type")]
        public string? BankAccountType { get; set; }

        [JsonPropertyName("bank_account_subtype")]
        public string? BankAccountSubtype { get; set; }

        [JsonPropertyName("bank_account_balance_current")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? BankAccountBalanceCurrent { get; set; }

        [JsonPropertyName("bank_account_balance_available")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? BankAccountBalanceAvailable { get; set; }

        [JsonPropertyName("bank_account_balance_limit")]
        [JsonConverter(typeof(NullableDecimalJsonConverter))]
        public decimal? BankAccountBalanceLimit { get; set; }

        [JsonPropertyName("deposit_account")]
        public bool? DepositAccount { get; set; }
    }
}