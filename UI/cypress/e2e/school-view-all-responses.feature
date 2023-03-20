Feature: View all responses page

    Scenario: View all responses page to have the expected structure
        Given a school clicks the magic link to respond to a view all responses
        And the user has arrived on the view all enquiry responses page
        Then the heading of the page is 'View responses to your tuition enquiry'
        And the page should have text stating the date and time the enquiry was made
        And there is text 'Your tuition requirements'
        When the user clicks Your tuition requirements
        Then The correct enquiry information is shown as follows:
            | Enquiry information                             |
            | This is the tuition requirements you asked for: |
            | Key stage 1: English and Maths                  |
            | Key stage 2: English and Maths                  |
            | Any                                             |
            | enquiry                                         |
            | enquiry                                         |
            | enquiry                                         |
        And there should be text stating the amount of tuition partners that responded

    Scenario: A tuition partners name and response date is shown on view all responses page
        Given a school clicks the magic link to respond to a view all responses
        And the user has arrived on the view all enquiry responses page
        Then The tuition partner responses are shown at the bottom of the page as follows:
            | Column          | Value                                 |
            | Date            | 20/03/2023                            |
            | Tuition Partner | Randstad Solutions                    |
            | Their Response  | View response from Randstad Solutions |

    Scenario: Clicking on a Tuition Partners Response redirects to their response page
        Given a school clicks the magic link to respond to a view all responses
        And the user has arrived on the view all enquiry responses page
        When they click View response from a tuition partner
        Then the tuition partners response page is shown