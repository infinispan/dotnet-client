if [ "$#" == "1" ]; then
  IFS=. read MAJ MIN MIC QUAL EXTRA <<< $(echo $1)
  echo Tagging for MAJOR=$MAJ MINOR=$MIN MICRO=$MIC QUAL=$QUAL
  if [ -n "$QUAL" ]; then
    PAT=$MIC.$QUAL
  else
    PAT=$MIC
  fi
  echo HOTROD_VERSION_MAJOR=$MAJ
  echo HOTROD_VERSION_MINOR=$MIN
  echo HOTROD_VERSION_PATCH=$MIC
  echo HOTROD_VERSION_LABEL=$QUAL
  if [ -n "$MAJ" ] && [ -n "$MIN" ] && [ -n "$PAT" ] && [ -z "$EXTRA" ]; then
    CURR_BRANCH=$(git rev-parse --abbrev-ref HEAD)
    git checkout -b __tmp
    sed -i -e 's/set (HOTROD_VERSION_MAJOR *".*")/set (HOTROD_VERSION_MAJOR "'"$MAJ"'")/' \
    -e 's/set (HOTROD_VERSION_MINOR *".*")/set (HOTROD_VERSION_MINOR "'"$MIN"'")/' \
    -e 's/set (HOTROD_VERSION_PATCH *".*")/set (HOTROD_VERSION_PATCH "'"$MIC"'")/' \
    -e 's/set (HOTROD_VERSION_LABEL *".*")/set (HOTROD_VERSION_LABEL "'"$QUAL"'")/' CMakeLists.txt
    sed -i -e 's/ branches: \[\[name: '\''master'\''/ branches: \[\[name: '\''refs\/tags\/'$MAJ.$MIN.$PAT\''/' Jenkinsfile
    sed -i "s/cppTag.*/cppTag = '$1'/" Jenkinsfile
    sed -i "s/version_1major.*/version_1major = '$MAJ'/" Jenkinsfile
    sed -i "s/version_2minor.*/version_2minor = '$MIN'/" Jenkinsfile
    sed -i "s/version_3micro.*/version_3micro = '$MIC'/" Jenkinsfile
    sed -i "s/version_4qualifier.*/version_4qualifier = '$QUAL'/" Jenkinsfile
    git add CMakeLists.txt Jenkinsfile
    git commit -m  "$MAJ.$MIN.$PAT"
    git tag -a "$MAJ.$MIN.$PAT" -m "$MAJ.$MIN.$PAT"
    git push origin "$MAJ.$MIN.$PAT"
    git checkout $CURR_BRANCH
    git branch -D __tmp
    exit 0
  fi
fi

echo "Usage: call me with #1 argument MAJ.MIN.MICRO[.QUALIF]"
echo "eg: $0 1.2.3.Fix"
echo -e "\t will create a tag '1.2.3.Fix' on both local and origin repo"
echo -e "\t CMakeFiles.txt will be updated according."
