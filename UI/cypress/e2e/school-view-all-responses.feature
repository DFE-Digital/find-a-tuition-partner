Feature: View all responses page

    Scenario: View all responses page to have the expected structure
        Given a school clicks the magic link to respond to a view all responses
        When the user has arrived on the view all enquiry responses page
        And the enquiry response page contains the same reference number
        Then the heading of the page is 'View responses to your tuition enquiry'
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
        When the user has arrived on the view all enquiry responses page
        Then The tuition partner responses are shown at the bottom of the page as follows:
            | Column          | Value                            |
            | Date            | 21/03/2023                       |
            | Tuition Partner | Sherpa Online                    |
            | Their Response  | View response from Sherpa Online |

    Scenario: Clicking on a Tuition Partners Response redirects to their response page
        Given a school clicks the magic link to respond to a view all responses
        When the user has arrived on the view all enquiry responses page
        And they click View response from a tuition partner
        Then the tuition partners response page is shown

    Scenario: Tuiton Partners Response Page has the expected structure
        Given a school clicks the magic link to respond to a view all responses
        When the user has arrived on the view all enquiry responses page
        And they click View response from a tuition partner
        When they click all the view your tuition requirement links the text shows
        Then the response page has the following information:
            | Requirement                    | Your Response                  | Sherpa Online's Response |
            | Key stage and subjects:        | Key stage 1: English and Maths | 80                       |
            | Key stage and subjects:        | Key stage 2: English and Maths | 80                       |
            | Tuition type:                  | Any                            | 80                       |
            | Tuition plan:                  | enquiry                        | 80                       |
            | Can you support SEND?:         | enquiry                        | 80                       |
            | Other tuition considerations?: | enquiry                        | 80                       |

    Scenario: Clicking View tuition partners contact details navigates to contact page for the tuition partner
        Given a school clicks the magic link to respond to a view all responses
        When the user has arrived on the view all enquiry responses page
        And they click View response from a tuition partner
        And they click contact tuition partner
        Then the user has arrived on the contact tuition partner page

    Scenario: Clicking cancel redirects to the view all responses page
        Given a school clicks the magic link to respond to a view all responses
        When the user has arrived on the view all enquiry responses page
        And they click View response from a tuition partner
        And they click cancel on the response page
        Then the user has arrived on the view all enquiry responses page

    Scenario: Clicking back redirects to the view all responses page
        Given a school clicks the magic link to respond to a view all responses
        When the user has arrived on the view all enquiry responses page
        And they click View response from a tuition partner
        And they click 'Back'
        Then the user has arrived on the view all enquiry responses page