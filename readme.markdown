RssMixxxer
==

It **composes** multiple feeds into one result feed.

It keeps **local cache** of source feeds to minimize network communication.

It serves a **configurable number of items** from source feeds, sorted by publish date.

It **periodically downloads** fresh copy of source feeds in background to make sure the result feed is up to date.

It **uses ETag/last-modified headers** to ensure that result feed is modified and recomposed only when any source changes.