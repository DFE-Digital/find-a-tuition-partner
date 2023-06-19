Feature: Schools Postcode confirmation in the Enquiry Journey

    Scenario: Postcode confirmation appears after the guidance page
        Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
        When they click 'Start now' button
        And the user will navigate to the guidance page
        And they click 'Continue' on enquiry
        Then they will be taken to the postcode confirmation page

    Scenario: Going back on Postcode confirmation takes you to the guidance page
        Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
        When they click 'Start now' button
        And the user will navigate to the guidance page
        And they click 'Continue' on enquiry
        Then they will be taken to the postcode confirmation page
        When they click 'Back'
        Then the user will navigate to the guidance page

    Scenario: User enters a postcode valid for more than one school and navigates to the postcode confirmation page
        Given user navigates to the postcode confirmation page with postcode 'B21 0RE'
        Then they will be taken to the school selection page
        Then they will see a list of schools with the same postcode
        And the user will select the first school in the list
        Then more information about the school selected will appear
        And the schools additional information shown will be:
            | Name and address                                                                   | Local authority | Phase of education | IDs                                              |
            | Oasis Academy Boulton Boulton Road, Handsworth, Birmingham, West Midlands, B21 0RE | Birmingham      | Primary            | URN: 139242 DfE number: 330/2117 UKPRN: 10040141 |
        When the user selects the second school in the list
        Then more information about the second school selected will appear
        And the second schools additional information shown will be:
            | Name and address                                                                 | Local authority | Phase of education | IDs                                              |
            | James Watt Primary School Boulton Road, Soho, Birmingham, West Midlands, B21 0RE | Birmingham      | Primary            | URN: 134102 DfE number: 330/2015 UKPRN: 10071778 |

    Scenario: User clicks I need to choose another school link in multiple school selection page
        Given user navigates to the postcode confirmation page with postcode 'B21 0RE'
        Then they will be taken to the school selection page
        Then they will see a list of schools with the same postcode
        Then the user clicks I need to choose another school link and continues
        Then they will be taken to the postcode confirmation page

    Scenario: User attempts to continue without selecting a school in multiple school selection page
        Given user navigates to the postcode confirmation page with postcode 'B21 0RE'
        Then they will be taken to the school selection page
        Then they will see a list of schools with the same postcode
        Then the user clicks continue
        Then they will see 'Select to confirm which school you work for' as an error message for the 'school selection'

    Scenario: User enters a postcode valid for one school and navigates to the postcode confirmation page
        Given user navigates to the postcode confirmation page with postcode 'OX42AU'
        Then they will be taken to the single school selection page
        And there is an option to confirm the school
        When the user clicks yes and continue
        Then user is redirected to the enter email address page

    Scenario: User clicks No I need to choose another school in single school selection page
        Given user navigates to the postcode confirmation page with postcode 'OX42AU'
        Then they will be taken to the single school selection page
        And there is an option to confirm the school
        When the user clicks No I need to choose another school and continue
        Then they will be taken to the postcode confirmation page

    Scenario: User navigates to the enquiry builder without a valid schools postcode
        Given user navigates to the postcode confirmation page with postcode 'SK1 1EB'
        Then they will be taken to the postcode confirmation page
        When the user clicks continue
        And they will see 'Enter a real school postcode' as an error message for the 'invalid schools postcode'



