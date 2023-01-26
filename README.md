# Get MedalTv Clips

1. Find your `userId` by going to your profile's page.  Open the developer tools (right-click -> inspect).  Search for `userId` on the page.  It's most likely going to be in a `<script>` tag with `var hydrationData`.  Otherwise, look in the networking tab and find it there.
2. Once you have the `userId`, download your content JSON file by going to `https://medal.tv/api/content?userId={userId}&limit=300000` (without the braces.)
3. Change the script to use your file.
4. `dotnet fsi get-medaltv-clips.fsx`