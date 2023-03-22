Feature: Contact us page from the view response page

    Scenario: Contact us page has the expected content structure
        Given a school clicks the magic link to respond to a view all responses
        When the user has arrived on the view all enquiry responses page
        And they click View response from a tuition partner
        And they click contact tuition partner
        Then the user has arrived on the contact tuition partner page
        And the heading of the page is 'Contact Sherpa Online'
        Then the page has the correct content information
        And the page shows contact information such as the following:
            | Section Name    | Value                    |
            | email:          | angela@sherpa-online.com |
            | contact number: | 01628 337 590            |

    Scenario: Clicking back redirects to tuition partner's response page
        Given a school clicks the magic link to respond to a view all responses
        When the user has arrived on the view all enquiry responses page
        And they click View response from a tuition partner
        And they click contact tuition partner
        Then the user has arrived on the contact tuition partner page
        When they click 'Back'
        Then the tuition partners response page is shown

    Scenario: Clicking return to enquiry list returns to all enquiries page
        Given a school clicks the magic link to respond to a view all responses
        When the user has arrived on the view all enquiry responses page
        And they click View response from a tuition partner
        And they click contact tuition partner
        Then the user has arrived on the contact tuition partner page
        When they click return to your enquiry list
        Then the user has arrived on the view all enquiry responses page


