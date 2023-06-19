Feature: User can view full details of a Tuition Parner

  Scenario: page title is 'Name of Tuition Partner'
    Given a user has arrived on the 'Tuition Partner' page for tp name 11
    Then the page's title is tp name 11

  Scenario: user clicks service name
    Given a user has started the 'Find a tuition partner' journey
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page

  Scenario: user directly accesses details page using the full name
    Given a user has arrived on the 'Tuition Partner' page for tp name 1
    Then the page URL ends with tp name 1
    And the heading should say tp name 1

  Scenario: user directly accesses details page using the SEO name
    Given a user has arrived on the 'Tuition Partner' page for tp name 12
    Then the page URL ends with tp name 12
    And the heading should say tp name 12

  Scenario: show Contact Details where TP has provided information
    Given a user has arrived on the 'Tuition Partner' page for tp name 12
    Then TP has provided full contact details

  Scenario: tuition partner details page linked from search results page has 'Back to search results' back link
    Given a user has arrived on the 'Tuition Partner' page for tp name 12 after searching for 'Key stage 1 English'
    Then the back link's text is 'Back to search results'

  Scenario: back is selected return to search results page
    Given a user has arrived on the 'Tuition Partner' page for tp name 12 after searching for 'Key stage 1 English'
    When they click 'Back'
    Then the page URL ends with '/search-results'
    And the search details include 'Key stage 1 English'

  Scenario: back is selected return to search results page with multiple subjects
    Given a user has arrived on the 'Tuition Partner' page for tp name 12 after searching for 'Key stage 1 English, Key stage 2 Maths'
    When they click 'Back'
    Then the page URL ends with '/search-results'
    And the search details include 'Key stage 1 English, Key stage 2 Maths'
  Scenario: user clicks quality assured tuition partner details summary
    Given a user has arrived on the 'Tuition Partner' page for tp name 12
    When they click 'How are tuition partners quality-assured?'
    Then the quality assured tuition partner details are shown
    And they will see the tribal link

  Scenario: tuition partner website link is displayed
    Given a user has arrived on the 'Tuition Partner' page for tp name 13
    Then the tuition partner's website link is displayed
    And the tuition partners website link starts with 'https://'

  Scenario: locations covered table is not displayed as default
    Given a user has arrived on the 'Tuition Partner' page for tp name 14
    Then the tuition partner locations covered table is not displayed

  Scenario: locations covered table is displayed when show-locations-covered=true
    Given a user has arrived on the 'Tuition Partner' page for tp name 14
    When they set the 'show-locations-covered' query string parameter value to 'true'
    Then the tuition partner locations covered table is displayed

  Scenario Outline: tuition cost table shows available tuition settings
    Given a user has arrived on the 'Tuition Partner' page for '<tuition-partner>'
    Then the tuition partner pricing table is displayed for '<tuition-settings>':
      | tuition-partner | tuition-settings |
      | tp name 14      | Online        |
      | tp name 15      | Online        |
      | tp name 16      | Face-to-face     |

    Examples:
      | tuition-partner | tuition-settings |
      | tp name 14      | Online        |
      | tp name 15      | Online        |
      | tp name 16      | Face-to-face     |


  Scenario: tuition cost blurb states pricing uniformity
    Given a user has arrived on the 'Tuition Partner' page for tp name 16
    Then the tuition cost information states declares no differences

  Scenario: full pricing tables are not displayed as default
    Given a user has arrived on the 'Tuition Partner' page for tp name 16
    Then the tuition partner full pricing tables are not displayed
    And the tuition partner pricing table is displayed

  Scenario: full pricing tables are displayed when show-full-pricing=true
    Given a user has arrived on the 'Tuition Partner' page for tp name 16
    When they set the 'show-full-pricing' query string parameter value to 'true'
    Then the tuition partner full pricing tables are displayed
    And the tuition partner pricing table is not displayed

  Scenario: meta data displayed when show-full-info=true
    Given a user has arrived on the 'Tuition Partner' page for tp name 16
    When they set the 'show-full-info' query string parameter value to 'true'
    Then the tuition partner full pricing tables are displayed
    And the tuition partner pricing table is not displayed
    And the tuition partner locations covered table is displayed
    And the tuition partner meta data is displayed

  Scenario: meta data is not displayed as default
    Given a user has arrived on the 'Tuition Partner' page for tp name 14
    Then the tuition partner meta data is not displayed

  Scenario: subjects covered by a tuition partner are in alphabetical order in the 'search results' page
    Given a user has arrived on the 'Tuition Partner' page for tp name 12
    Then the subjects covered by a tuition partner are in alphabetical order

  Scenario: Tuition partner details are displayed correctly when arriving on the results page
    Given a user has arrived on the 'Tuition Partner' page for tp name 12
    Then all tuition partner details are populated correctly

  Scenario: Logos are displayed for tuition partners
    Given a user has arrived on the 'Tuition Partner' page for tp name 15
    Then the logo is shown

  Scenario: Logos are not displayed for tuition partners
    Given a user is using a 'phone'
    Given a user has arrived on the 'Tuition Partner' page for tp name 15
    Then the logo is not shown

  Scenario: No LA label shown if go to TP details via All TP pages
    Given a user has arrived on the 'Tuition Partner' page for tp name 12
    Then the LA name is not shown

  Scenario: LA label shown if go to TP details via search results page
    Given a user has arrived on the 'Tuition Partner' page for tp 12 after searching for 'Key stage 1 English' in postcode 'sk11eb'
    Then the LA name displayed is 'Stockport'

  Scenario: LA span shows correct the text if TP details page is gotten to via search results page
    Given a user has arrived on the 'Tuition Partner' page for tp 12 after searching for 'Key stage 1 English' in postcode 'sk11eb'
    Then  the LA label text is 'Tuition partner for Stockport'

  Scenario: A user should be able to add and remove TP to and from the price comparison list
    Given a user has arrived on the 'Tuition Partner' page for tp 12 after searching for 'Key stage 1 English' in postcode 'sk11eb'
    And tp name 12 checkbox is unchecked on its detail page
    And the LA label text is 'Tuition partner for Stockport'
    Then the user checks the tp name 12 checkbox on its detail page
    And  tp name 12 checkbox is checked on its detail page
    And the LA label text is 'Price comparison listed tuition partner for Stockport'
    Then the user unchecks the tp name 12 checkbox
    And tp name 12 checkbox is unchecked on its detail page
    And the LA label text is 'Tuition partner for Stockport'

  Scenario: Adding or removing TP to price comparison list from TP details page should be reflected on the search results page
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English' for postcode 'sk11eb'
    And total amount of price comparison list TPs is 0
    And tp name 9 checkbox is unchecked
    Then tp name 9 name link is clicked
    And tp name 9 checkbox is unchecked on its detail page
    And the LA label text is 'Tuition partner for Stockport'
    Then the user checks the tp name 9 checkbox on its detail page
    And  tp name 9 checkbox is checked on its detail page
    And the LA label text is 'Price comparison listed tuition partner for Stockport'
    Then they click Back to go back to the search results page
    And total amount of price comparison list TPs is 1
    And the 'TuitionPartner' - tp name 9 checkbox is checked
    Then tp name 9 name link is clicked
    And  tp name 9 checkbox is checked on its detail page
    And the LA label text is 'Price comparison listed tuition partner for Stockport'
    Then the user unchecks the tp name 9 checkbox
    And tp name 9 checkbox is unchecked on its detail page
    And the LA label text is 'Tuition partner for Stockport'
    Then they click Back to go back to the search results page
    And total amount of price comparison list TPs is 0
    And the 'TuitionPartner' - tp name 9 checkbox is unchecked