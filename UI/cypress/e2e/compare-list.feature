Feature: Tuition Partner price comparison list

  Scenario: User can add a TP to their price comparison list from the results page
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    When they add 'Action Tutoring' to their price comparison list on the results page
    Then 'Action Tutoring' is marked as added to the price comparison list on the results page
    And the price comparison list shows as having 1 entries on the results page

  Scenario: User can add multiple TPs to their price comparison list from the results page
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    When they add 'Action Tutoring' to their price comparison list on the results page
    And they add 'm2r Education' to their price comparison list on the results page
    Then 'Action Tutoring' is marked as added to the price comparison list on the results page
    And 'm2r Education' is marked as added to the price comparison list on the results page
    And the price comparison list shows as having 2 entries on the results page

  Scenario: User can add lots of TPs to their price comparison list in quick succession from the results page
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    When they programmatically add the first 20 results to their price comparison list on the results page
    Then the price comparison list shows as having 20 entries on the results page

  Scenario: User can remove a TP from their price comparison list from the results page
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Action Tutoring' to their price comparison list on the results page
    When they remove 'Action Tutoring' from their price comparison list on the results page
    Then 'Action Tutoring' is not marked as added to the price comparison list on the results page
    And the price comparison list shows as having 0 entries on the results page

  Scenario: User goes straight to their empty price comparison list page
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    When they choose to view their price comparison list from the results page
    Then there are 0 entries on the price comparison list page

  Scenario: Price comparison list back button takes the user back to the search results
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they choose to view their price comparison list from the results page
    When they click 'Back'
    Then the search results are displayed

  Scenario: on the price comparison list page blank postcode user redirect back to the search results page with a validation error shown
    Given a user has arrived on the 'Compare tuition partner prices' page for postcode ''
    Then the page URL ends with '/search-results'
    And they will see 'Enter a postcode' as an error message for the 'postcode'

  Scenario: on the price comparison list page invalid postcode user redirect back to the search results page with a validation error shown
    Given a user has arrived on the 'Compare tuition partner prices' page for postcode 'invalid postcode'
    Then the page URL ends with '/search-results'
    And they will see 'Enter a real postcode' as an error message for the 'postcode'

  Scenario: User views their price comparison listed TPs on the price comparison list page
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add tp name 1 to their price comparison list on the results page
    And they add tp name 2 to their price comparison list on the results page
    When they choose to view their price comparison list from the results page
    Then there are 2 entries on the price comparison list page
    And the heading caption is 'Tuition partners for Stockport'
    And tp name is entry 1 on the price comparison list page
    And tp name is entry 2 on the price comparison list page

  Scenario: User changes their filters to exclude a price comparison listed TP
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Action Tutoring' to their price comparison list on the results page
    And they add 'EM Tuition' to their price comparison list on the results page
    When they click on the option heading for 'Key stage 4'
    And they select subject 'key-stage-4-humanities'
    Then the price comparison list shows as having 2 entries on the results page

  Scenario: User changes their postcode to exclude a price comparison listed TP
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Action Tutoring' to their price comparison list on the results page
    And they add 'EM Tuition' to their price comparison list on the results page
    When they enter 'TN22 2BL' as the school's postcode
    And they click 'Search'
    Then the price comparison list shows as having 2 entries on the results page
    And they choose to view their price comparison list from the results page
    And there are 1 entries on the price comparison list page
    And the heading caption is 'Tuition partner for Wealden'
    And 'EM Tuition' is entry 1 on the price comparison list page
    And 'Action Tutoring' is entry 1 on the not available list on the price comparison list page

  Scenario: The price comparison list displays the expected data for the search area
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'W1W 7RT'
    And they add 'Reeson Education' to their price comparison list on the results page
    When they choose to view their price comparison list from the results page
    Then there are 1 entries on the price comparison list page
    And entry 1 on the price comparison list is the row 'Reeson Education', '1 to 1, 2, 3, 4, 5, 6', 'In School, Online', '£8.33 to £50 excluding VAT'
    And they click 'Back'
    And they enter 'TN22 2BL' as the school's postcode
    And they click 'Search'
    And they choose to view their price comparison list from the results page
    And entry 1 on the price comparison list is the row 'Reeson Education', '1 to 1, 2, 3, 4, 5, 6', 'Online', '£8.33 to £50 excluding VAT'

  Scenario: User views TP details from the price comparison list
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Action Tutoring' to their price comparison list on the results page
    And they choose to view their price comparison list from the results page
    When they choose to view the 'Action Tutoring' details from the price comparison list
    Then the back link's text is 'Back to price comparison list'
    And the heading caption is 'Price comparison listed tuition partner for Stockport'
    And they click 'Back'
    And they will be taken to the 'Price comparison list' page

  Scenario: Default price comparison list sort is by order selected to be added
    Given a user has selected TPs to add to their price comparison list and journeyed forward to the price comparison list page
    And 'Reeson Education' is entry 1 on the price comparison list page
    And 'Action Tutoring' is entry 2 on the price comparison list page
    And 'EM Tuition' is entry 3 on the price comparison list page
    And 'Booster Club' is entry 4 on the price comparison list page
    And 'Equal Education' is entry 5 on the price comparison list page
    And 'Tutors Green' is entry 6 on the price comparison list page

  Scenario: User sorts price comparison list table by Price, uses min price for ascending, max price for descending
    Given a user has selected TPs to add to their price comparison list and journeyed forward to the price comparison list page
    Then they choose to sort the price comparison list by price
    And 'Reeson Education' is entry 1 on the price comparison list page
    And 'EM Tuition' is entry 2 on the price comparison list page
    And 'Tutors Green' is entry 3 on the price comparison list page
    And 'Booster Club' is entry 4 on the price comparison list page
    And 'Action Tutoring' is entry 5 on the price comparison list page
    And 'Equal Education' is entry 6 on the price comparison list page
    Then they choose to sort the price comparison list by price
    And 'Equal Education' is entry 1 on the price comparison list page
    And 'Reeson Education' is entry 2 on the price comparison list page
    And 'Tutors Green' is entry 3 on the price comparison list page
    And 'EM Tuition' is entry 4 on the price comparison list page
    And 'Action Tutoring' is entry 5 on the price comparison list page
    And 'Booster Club' is entry 6 on the price comparison list page

  Scenario: User removes single item from price comparison list
    Given a user has selected TPs to add to their price comparison list and journeyed forward to the price comparison list page
    Then they choose to remove entry 2 on the price comparison list page
    Then there are 5 entries on the price comparison list page
    And 'Reeson Education' is entry 1 on the price comparison list page
    And 'EM Tuition' is entry 2 on the price comparison list page
    Then they click 'Back'
    And the search results are displayed
    Then they choose to view their price comparison list from the results page
    Then there are 5 entries on the price comparison list page
    And 'Reeson Education' is entry 1 on the price comparison list page
    And 'EM Tuition' is entry 2 on the price comparison list page

  Scenario: User clears full price comparison list then cancel
    Given a user has selected TPs to add to their price comparison list and journeyed forward to the price comparison list page
    Then they choose to click on clear price comparison list link
    And they are taken to the clear price comparison list confirmation page
    Then they click the cancel link
    Then there are 6 entries on the price comparison list page

  Scenario: User clears full price comparison list
    Given a user has selected TPs to add to their price comparison list and journeyed forward to the price comparison list page
    Then they choose to click on clear price comparison list link
    And they are taken to the clear price comparison list confirmation page
    Then they click confirm button
    Then there are 0 entries on the price comparison list page

  Scenario: Adding or removing TP to price comparison list from search results page should be reflected on the TP details page
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English' for postcode 'sk11eb'
    And total amount of price comparison list TPs is 0
    And 'Seven Springs Education' checkbox is unchecked
    Then 'Seven Springs Education' name link is clicked
    And 'Seven Springs Education' checkbox is unchecked on its detail page
    And the LA label text is 'Tuition partner for Stockport'
    Then they click 'Back'
    Then they add 'Seven Springs Education' to their price comparison list on the results page
    And total amount of price comparison list TPs is 1
    Then 'Seven Springs Education' name link is clicked
    And  'Seven Springs Education' checkbox is checked on its detail page
    And the LA label text is 'Price comparison listed tuition partner for Stockport'
    Then they click 'Back'
    Then they remove 'Seven Springs Education' from their price comparison list on the results page
    And total amount of price comparison list TPs is 0
    Then 'Seven Springs Education' name link is clicked
    And 'Seven Springs Education' checkbox is unchecked on its detail page
    And the LA label text is 'Tuition partner for Stockport'

  Scenario: Removing last price comparison listed TP from TP details page is reflected in price comparison list page
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Reeson Education' to their price comparison list on the results page
    When they choose to view their price comparison list from the results page
    Then there is 1 entry on the price comparison list page
    And 'Reeson Education' is entry 1 on the price comparison list page
    Then 'Reeson Education' name link is clicked
    And 'Reeson Education' is removed from the price comparison list
    Then they click Back to go back to the price comparison list page
    And the price comparison list page displays 'You don’t have any tuition partners selected to compare.'

  Scenario: The price comparison list can be refined by group size then TP rows and the price are updated
    Given a user has selected TPs to add to their price comparison list and journeyed forward to the price comparison list page
    Then the 'Reeson Education' price is '£8.33 to £50 excluding VAT'
    When '1 to 1' group size price comparison list refinement option is selected
    Then 'Reeson Education' is entry 1 on the price comparison list page
    And 'EM Tuition' is entry 2 on the price comparison list page
    And 'Equal Education' is entry 3 on the price comparison list page
    And 'Tutors Green' is entry 4 on the price comparison list page
    And 'Action Tutoring' is entry 5 on the price comparison list page
    And 'Booster Club' is entry 6 on the price comparison list page
    And the 'Reeson Education' price is '£50 excluding VAT'

  Scenario: The price comparison list can be refined by tuition type then TP rows and the price are updated
    Given a user has selected TPs to add to their price comparison list and journeyed forward to the price comparison list page
    Then the 'Tutors Green' price is '£8.68 to £45.83 excluding VAT'
    When 'In School' tuition type price comparison list refinement option is selected
    And 'EM Tuition' is entry 1 on the price comparison list page
    And 'Booster Club' is entry 2 on the price comparison list page
    And 'Equal Education' is entry 3 on the price comparison list page
    And 'Tutors Green' is entry 4 on the price comparison list page
    And 'Reeson Education' is entry 5 on the price comparison list page
    Then 'Action Tutoring' is entry 6 on the price comparison list page
    And the 'Tutors Green' price is '	£9.38 to £45.83 excluding VAT'

  Scenario: The price comparison list can be refined by group size and tuition type then TP rows and the price are updated
    Given a user has selected TPs to add to their price comparison list and journeyed forward to the price comparison list page
    Then the 'Tutors Green' price is '£8.68 to £45.83 excluding VAT'
    When '1 to 1' group size price comparison list refinement option is selected
    And 'Online' tuition type price comparison list refinement option is selected
    Then 'Reeson Education' is entry 1 on the price comparison list page
    And 'EM Tuition' is entry 2 on the price comparison list page
    And 'Equal Education' is entry 3 on the price comparison list page
    And 'Tutors Green' is entry 4 on the price comparison list page
    And 'Action Tutoring' is entry 5 on the price comparison list page
    And 'Booster Club' is entry 6 on the price comparison list page
    And the 'Tutors Green' price is '	£41.67 excluding VAT'

  Scenario: The price comparison list price ordering works with refined data and any inavid TP data still follows the order they were added to price comparison list
    Given a user has selected TPs to add to their price comparison list and journeyed forward to the price comparison list page
    When '1 to 1' group size price comparison list refinement option is selected
    And 'Online' tuition type price comparison list refinement option is selected
    And they choose to sort the price comparison list by price
    Then 'EM Tuition' is entry 1 on the price comparison list page
    And 'Tutors Green' is entry 2 on the price comparison list page
    And 'Reeson Education' is entry 3 on the price comparison list page
    And 'Equal Education' is entry 4 on the price comparison list page
    And 'Action Tutoring' is entry 5 on the price comparison list page
    And 'Booster Club' is entry 6 on the price comparison list page
    And they choose to sort the price comparison list by price
    Then 'Equal Education' is entry 1 on the price comparison list page
    And 'Reeson Education' is entry 2 on the price comparison list page
    And 'Tutors Green' is entry 3 on the price comparison list page
    And 'EM Tuition' is entry 4 on the price comparison list page
    And 'Action Tutoring' is entry 5 on the price comparison list page
    And 'Booster Club' is entry 6 on the price comparison list page

  Scenario: The price comparison list refinement shows the correct message against a TP if no data is returned
    Given a user has selected TPs to add to their price comparison list and journeyed forward to the price comparison list page
    When '1 to 1' group size price comparison list refinement option is selected
    Then the 'Action Tutoring' empty data reason is 'Does not offer group sizes of 1 to 1'
    When 'Any' group size price comparison list refinement option is selected
    And 'In School' tuition type price comparison list refinement option is selected
    Then the 'Action Tutoring' empty data reason is 'Does not offer in school tuition in Stockport'
    When '1 to 1' group size price comparison list refinement option is selected
    Then the 'Action Tutoring' empty data reason is 'Does not offer group sizes of 1 to 1 or in school tuition in Stockport'

  Scenario: The price comparison list refinement tuition type defaults to the search tuition type filter on first load and then maintains own state after being selected until the search tuition type filter is changed
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB' and tuition type 'Any'
    And they add 'Action Tutoring' to their price comparison list on the results page
    When they choose to view their price comparison list from the results page
    Then the tuition type select option is 'Any'
    And the group size select option is 'Any'
    When they click 'Back'
    Then the search results are displayed
    When the user selects tuition type 'in school'
    And they choose to view their price comparison list from the results page
    Then the tuition type select option is 'In School'
    And the group size select option is 'Any'
    When 'Online' tuition type price comparison list refinement option is selected
    Then the tuition type select option is 'Online'
    When they click 'Back'
    Then the search results are displayed
    When they choose to view their price comparison list from the results page
    Then the tuition type select option is 'Online'
    When they click 'Back'
    Then the search results are displayed
    When the user selects tuition type 'any'
    When they choose to view their price comparison list from the results page
    Then the tuition type select option is 'Any'

  Scenario: The price comparison list page shows the previously searched by subjects
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English, Key stage 2 Maths, Key stage 3 Maths' for postcode 'SK1 1EB'
    And they add 'Action Tutoring' to their price comparison list on the results page
    When they choose to view their price comparison list from the results page
    Then the price comparison list key stage subjects label number 1 is 'Key stage 2: English and Maths'
    Then the price comparison list key stage subjects label number 2 is 'Key stage 3: Maths'

  Scenario: The price comparison list page shows no message if no subjects have been searched for
    Given a user has arrived on the 'Search results' page without subjects
    And they add 'Action Tutoring' to their price comparison list on the results page
    When they choose to view their price comparison list from the results page
    Then the price comparison list key stage subjects header is not shown

  Scenario: The price comparison list displays VAT is not applicable if needed
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Action Tutoring' to their price comparison list on the results page
    When they choose to view their price comparison list from the results page
    Then there are 1 entries on the price comparison list page
    And entry 1 on the price comparison list is the row 'Action Tutoring', '1 to 2', 'Online', '£20.12 VAT does not apply'

  Scenario: The price comparison list can show VAT inclusive and then toggle to VAT exclusive prices
    Given a user has selected TPs to add to their price comparison list and journeyed forward to the price comparison list page
    Then the 'Tutors Green' price is '£8.68 to £45.83 excluding VAT'
    Then the 'Action Tutoring' price is '£20.12 VAT does not apply'
    When 'Show prices including VAT' VAT price comparison list refinement option is selected
    Then the 'Tutors Green' price is '£10.42 to £55 including VAT'
    And the 'Action Tutoring' price is '£20.12 VAT does not apply'

  Scenario: The price comparison list price ordering works with inclusive or exclusive prices
    Given a user has selected TPs to add to their price comparison list and journeyed forward to the price comparison list page
    Then they choose to sort the price comparison list by price
    And 'Reeson Education' is entry 1 on the price comparison list page
    And 'EM Tuition' is entry 2 on the price comparison list page
    And 'Tutors Green' is entry 3 on the price comparison list page
    And 'Booster Club' is entry 4 on the price comparison list page
    And 'Action Tutoring' is entry 5 on the price comparison list page
    And 'Equal Education' is entry 6 on the price comparison list page
    When 'Show prices including VAT' VAT price comparison list refinement option is selected
    Then 'EM Tuition' is entry 1 on the price comparison list page
    And 'Reeson Education' is entry 2 on the price comparison list page
    And 'Tutors Green' is entry 3 on the price comparison list page
    And 'Action Tutoring' is entry 4 on the price comparison list page
    And 'Booster Club' is entry 5 on the price comparison list page
    And 'Equal Education' is entry 6 on the price comparison list page

  Scenario: The price comparison list VAT defaults to inclusive on first load and then maintains own state after being selected
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Action Tutoring' to their price comparison list on the results page
    When they choose to view their price comparison list from the results page
    Then the VAT select option is 'Show prices excluding VAT'
    When 'Show prices including VAT' VAT price comparison list refinement option is selected
    Then the VAT select option is 'Show prices including VAT'
    When they click 'Back'
    Then the search results are displayed
    When they choose to view their price comparison list from the results page
    Then the VAT select option is 'Show prices including VAT'


  Scenario: The price comparison list shows the Local Authority District for the request
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And a user has selected TPs to add to their price comparison list and journeyed forward to the price comparison list page
    Then the correct Local Authority District is shown for 'Stockport'

  Scenario: The price comparison list displays VAT is not applicable if needed
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'YO11 1BA'
    And they add 'Randstad Solutions' to their price comparison list on the results page
    When they choose to view their price comparison list from the results page
    Then there are 1 entries on the price comparison list page
    Then the correct Local Authority District is shown for 'Scarborough'