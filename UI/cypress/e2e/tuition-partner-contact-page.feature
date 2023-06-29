Feature: View all responses page and Tuition partner contact page

    Scenario: Submitting a successful response shows the confirmation page correctly
        Given a user has started the 'Find a tuition partner' journey
        When a tuition partner clicks the magic link to respond to a schools enquiry and progresses to edit response
        Then Tuition partner has journeyed to the check your answers page
        When  the user has arrived on the tuition response check your answers page
        And they click 'Submit'
        Then the user has arrived on the tuition response confirmation page
        And the page has title 'Response sent'
        And a unique reference number is shown


    Scenario: View all responses page to have the expected structure
        Given a school clicks the magic link to view their enquiry
        When the user has arrived on the view all enquiry responses page
        Then the heading of the page has text 'View responses to your tuition enquiry'
        And there is text 'Your tuition requirements'
        When the user clicks Your tuition requirements
        Then The correct enquiry information is shown as follows:
            | Question                     | Your Requirements              |
            | Key stage and subjects       | Key stage 1: English and Maths |
            | Key stage and subjects       | Key stage 2: English and Maths |
            | Tuition setting              | No preference                  |
            | Tuition plan                 | enquiry                        |
            | Can you support SEND         | enquiry                        |
            | Other tuition considerations | enquiry                        |


    Scenario: A tuition partners name and response date is shown on view all responses page
        Given a school clicks the magic link to view their enquiry
        Then The tuition partner responses are shown at the bottom of the page as follows:
            | Column                   | Value       |
            | Date                     |             |
            | Tuition Partner Response | {TP's name} |
            | Your interest            | UNREAD      |


    Scenario: Clicking on a Tuition Partners Response redirects to their response page
        Given a school clicks the magic link to view their enquiry
        Then they click View response from a tuition partner
        Then the tuition partners response page is shown

    Scenario: Tuiton Partners Response Page has the expected structure
        Given a school clicks the magic link to view their enquiry
        Then they click View response from a tuition partner
        Then the response page has the following information:
            | Requirement                    | Your Response                  | {TP's name}'s Response |
            | Key stage and subjects:        | Key stage 1: English and Maths | 80                     |
            | Key stage and subjects:        | Key stage 2: English and Maths | 80                     |
            | Tuition setting:               | No preference                  | 80                     |
            | Tuition plan:                  | enquiry                        | 80                     |
            | Can you support SEND?:         | enquiry                        | 80                     |
            | Other tuition considerations?: | enquiry                        | 80                     |


    Scenario: Contact us page has the expected content structure
        Given a school clicks the magic link to view their enquiry
        Then they click View response from a tuition partner
        When the school clicks 'I’m interested, contact tuition partner'
        Then the user has arrived on the contact tuition partner page
        Then the page has the correct content information


    Scenario: Clicking back redirects to tuition partner's response page
        Given a school clicks the magic link to view their enquiry
        Then they click View response from a tuition partner
        When the school clicks 'I’m interested, contact tuition partner'
        Then the user has arrived on the contact tuition partner page

    Scenario: Clicking return to enquiry list returns to all enquiries page
        Given a school clicks the magic link to view their enquiry
        Then they click View response from a tuition partner
        When the school clicks 'I’m interested, contact tuition partner'
        Then the user has arrived on the contact tuition partner page



