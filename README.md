# suggestions
  A simple suggestion service for city names

Technologies:
  - Asp.net Web API
  - Asp.net MVC
  - Trie
  - Strategy pattern
  - Twitter Bootstrap

Usage:
  Query:
  http://<host>/suggestion?q=Londo&latitude=43.70011&longitude=-79.4163
  - lat and long are optional
  - can be queried either by "lat" or "latitude"
  - can be queried either by "long" or "longitude"
  - lat and long are optional

  Response:
  [
      {
        "name": "London, ON, Canada",
        "latitude": "42.98339",
        "longitude": "-81.23304",
        "score": 0.9
      },
      {
        "name": "London, OH, USA",
        "latitude": "39.88645",
        "longitude": "-83.44825",
        "score": 0.5
      },
  ]
  
Future improvements:
  - Unit tests
  - Better algorithm for string matching (now it uses LCS)
  - AI: record user responses to the db alongside the original data, feed to a ML algorithm and build a prediction model
